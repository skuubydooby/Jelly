# Usage: .\rename.ps1 -OldName "topping" -NewName "Example" [-ProjectPath "."] [-DryRun]

param(
    [Parameter(Mandatory=$true)]
    [string]$OldName,

    [Parameter(Mandatory=$true)]
    [string]$NewName,

    [string]$ProjectPath = ".",

    [switch]$DryRun
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Intelligent Complete Rename Tool" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Old Name: $OldName" -ForegroundColor Yellow
Write-Host "New Name: $NewName" -ForegroundColor Green
Write-Host "Scanning: $ProjectPath" -ForegroundColor White
if ($DryRun) {
    Write-Host "MODE: DRY RUN (no changes will be made)" -ForegroundColor Magenta
}
Write-Host ""

$stats = @{
    FilesContentUpdated = 0
    FilesRenamed = 0
    FoldersRenamed = 0
    TotalReplacements = 0
}

# Resolve the full path
$ProjectPath = Resolve-Path $ProjectPath

# Step 1: Update content in ALL text-based files
Write-Host "Step 1: Updating file contents recursively..." -ForegroundColor Cyan
$textExtensions = @("*.cs", "*.csproj", "*.sln", "*.json", "*.xml", "*.config", "*.txt", "*.props", "*.targets", "*.resx", "*.xaml", "*.py", "*.yml", "*.yaml", "*.sh", "*.md", "*.dockerfile", "*.bat", "*.ps1")
$allFiles = @()

foreach ($ext in $textExtensions) {
    $allFiles += Get-ChildItem -Path $ProjectPath -Recurse -Filter $ext -File -ErrorAction SilentlyContinue
}

# Add files without extensions (like Dockerfile, Makefile, etc.)
$noExtensionFiles = Get-ChildItem -Path $ProjectPath -Recurse -File -ErrorAction SilentlyContinue | Where-Object { -not $_.Extension }
$allFiles += $noExtensionFiles

Write-Host "Found $($allFiles.Count) files to check..." -ForegroundColor White

foreach ($file in $allFiles) {
    try {
        # Detect encoding
        $encoding = [System.Text.Encoding]::UTF8
        $bytes = [System.IO.File]::ReadAllBytes($file.FullName)

        # Check for BOM
        if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
            $encoding = New-Object System.Text.UTF8Encoding($true)
        } elseif ($bytes.Length -ge 2 -and $bytes[0] -eq 0xFF -and $bytes[1] -eq 0xFE) {
            $encoding = [System.Text.Encoding]::Unicode
        }

        $content = [System.IO.File]::ReadAllText($file.FullName, $encoding)
        $originalContent = $content

        # Create case-sensitive and case-insensitive patterns
        $oldNameLower = $OldName.ToLower()
        $newNameLower = $NewName.ToLower()
        
        # Count total occurrences
        $matches = ([regex]::Matches($content, [regex]::Escape($OldName), [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)).Count

        if ($matches -gt 0) {
            # Replace exact case matches (namespace, class names, etc.)
            $content = $content -creplace "\b$OldName\b", $NewName
            
            # Replace PascalCase variations (e.g., toppingManager -> ExampleManager)
            $content = $content -creplace "\b$OldName([A-Z][a-zA-Z0-9]*)", "$NewName`$1"
            
            # Replace camelCase variations (e.g., toppingManager -> exampleManager)
            $content = $content -creplace "\b$oldNameLower([A-Z][a-zA-Z0-9]*)", "$newNameLower`$1"
            
            # Replace in namespaces
            $content = $content -replace "namespace\s+$OldName\b", "namespace $NewName"
            $content = $content -replace "namespace\s+([a-zA-Z0-9_.]+\.)$OldName\b", "namespace `$1$NewName"
            
            # Replace using statements
            $content = $content -replace "using\s+$OldName\b", "using $NewName"
            $content = $content -replace "using\s+([a-zA-Z0-9_.]+\.)$OldName\b", "using `$1$NewName"
            
            # Replace in paths (Windows and Unix style)
            $content = $content -replace "\\$OldName\\", "\$NewName\"
            $content = $content -replace "/$OldName/", "/$NewName/"
            $content = $content -replace "\\$OldName\b", "\$NewName"
            $content = $content -replace "/$OldName\b", "/$NewName"
            
            # Replace project references
            $content = $content -replace """$OldName""", """$NewName"""
            $content = $content -replace "'$OldName'", "'$NewName'"
            $content = $content -replace """$OldName\.csproj""", """$NewName.csproj"""
            
            # Replace assembly and namespace names in XML
            $content = $content -replace "<AssemblyName>$OldName</AssemblyName>", "<AssemblyName>$NewName</AssemblyName>"
            $content = $content -replace "<RootNamespace>$OldName</RootNamespace>", "<RootNamespace>$NewName</RootNamespace>"
            $content = $content -replace "<ProjectReference Include="".*\\$OldName\\$OldName\.csproj""", "<ProjectReference Include=""..\$NewName\$NewName.csproj"""
            
            # Replace with dot notation (e.g., topping.Techniques)
            $content = $content -creplace "\b$OldName\.", "$NewName."
            
            if ($content -ne $originalContent) {
                if (-not $DryRun) {
                    [System.IO.File]::WriteAllText($file.FullName, $content, $encoding)
                }
                $stats.FilesContentUpdated++
                $stats.TotalReplacements += $matches
                $relativePath = $file.FullName.Replace($ProjectPath, '.')
                Write-Host "  Updated: $relativePath - $matches replacements" -ForegroundColor Green
            }
        }
    } catch {
        Write-Host "  Warning: Could not process $($file.Name) - $_" -ForegroundColor Yellow
    }
}

# Step 2: Rename all FILES containing the old name (recursively, from deepest first)
Write-Host ""
Write-Host "Step 2: Renaming files with '$OldName' in filename..." -ForegroundColor Cyan
$filesToRename = Get-ChildItem -Path $ProjectPath -Recurse -File -ErrorAction SilentlyContinue | 
    Where-Object { $_.Name -match [regex]::Escape($OldName) } |
    Sort-Object { $_.FullName.Length } -Descending

foreach ($file in $filesToRename) {
    $newFileName = $file.Name -replace [regex]::Escape($OldName), $NewName
    $newFilePath = Join-Path $file.Directory.FullName $newFileName

    if ($DryRun) {
        Write-Host "  [DRY RUN] Would rename: $($file.FullName) -> $newFileName" -ForegroundColor Yellow
        $stats.FilesRenamed++
    } else {
        if (Test-Path $newFilePath) {
            Write-Host "  Skipping: $($file.Name) - target exists" -ForegroundColor Yellow
        } else {
            try {
                Rename-Item -Path $file.FullName -NewName $newFileName -ErrorAction Stop
                Write-Host "  Renamed: $($file.Name) -> $newFileName" -ForegroundColor Green
                $stats.FilesRenamed++
            } catch {
                Write-Host "  Error: $($file.Name) - $_" -ForegroundColor Red
            }
        }
    }
}

# Step 3: Rename all FOLDERS containing the old name (bottom-up)
Write-Host ""
Write-Host "Step 3: Renaming folders with '$OldName' in name..." -ForegroundColor Cyan
$foldersToRename = Get-ChildItem -Path $ProjectPath -Recurse -Directory -ErrorAction SilentlyContinue | 
    Where-Object { $_.Name -match [regex]::Escape($OldName) } | 
    Sort-Object { $_.FullName.Length } -Descending

foreach ($folder in $foldersToRename) {
    $newFolderName = $folder.Name -replace [regex]::Escape($OldName), $NewName
    $newFolderPath = Join-Path $folder.Parent.FullName $newFolderName

    if ($DryRun) {
        Write-Host "  [DRY RUN] Would rename: $($folder.FullName) -> $newFolderName" -ForegroundColor Yellow
        $stats.FoldersRenamed++
    } else {
        if (Test-Path $newFolderPath) {
            Write-Host "  Skipping: $($folder.Name) - target exists" -ForegroundColor Yellow
        } else {
            try {
                Rename-Item -Path $folder.FullName -NewName $newFolderName -ErrorAction Stop
                Write-Host "  Renamed: $($folder.FullName) -> $newFolderName" -ForegroundColor Green
                $stats.FoldersRenamed++
            } catch {
                Write-Host "  Error: $($folder.Name) - $_" -ForegroundColor Red
                Write-Host "  Close all programs accessing this folder and try again" -ForegroundColor Yellow
            }
        }
    }
}

# Summary
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Summary:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "File Contents Updated: $($stats.FilesContentUpdated)" -ForegroundColor Green
Write-Host "Total Text Replacements: $($stats.TotalReplacements)" -ForegroundColor Green
Write-Host "Files Renamed: $($stats.FilesRenamed)" -ForegroundColor Green
Write-Host "Folders Renamed: $($stats.FoldersRenamed)" -ForegroundColor Green

if ($DryRun) {
    Write-Host ""
    Write-Host "This was a DRY RUN. No actual changes were made." -ForegroundColor Magenta
    Write-Host "Run without -DryRun to apply changes." -ForegroundColor Magenta
} else {
    Write-Host ""
    Write-Host "Rename complete!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "1. Build your solution: dotnet build" -ForegroundColor White
    Write-Host "2. Check for errors and test" -ForegroundColor White
    Write-Host "3. Update any external references to the old name" -ForegroundColor White
}

Write-Host ""