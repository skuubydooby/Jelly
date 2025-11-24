# Intelligent Complete Renaming Script
# Recursively finds and renames EVERYTHING - files, folders, content
# Usage: .\rename.ps1 -OldName "Interop" -NewName "Interop"

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

# Step 1: Update content in ALL text-based files
Write-Host "Step 1: Updating file contents recursively..." -ForegroundColor Cyan
$textExtensions = @("*.cs", "*.csproj", "*.sln", "*.json", "*.xml", "*.config", "*.txt", "*.props", "*.targets", "*.resx", "*.xaml")
$allFiles = @()

foreach ($ext in $textExtensions) {
    $allFiles += Get-ChildItem -Path $ProjectPath -Recurse -Filter $ext -File -ErrorAction SilentlyContinue
}

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
        
        # Count how many times the old name appears
        $matches = ([regex]::Matches($content, "\b$OldName\b")).Count
        
        if ($matches -gt 0) {
            # Replace namespace declarations
            $content = $content -replace "namespace\s+$OldName\b", "namespace $NewName"
            
            # Replace using statements
            $content = $content -replace "using\s+$OldName\b", "using $NewName"
            
            # Replace in paths
            $content = $content -replace "\\$OldName\\", "\$NewName\"
            $content = $content -replace "$OldName\\", "$NewName\"
            $content = $content -replace "\\$OldName\.", "\$NewName."
            
            # Replace project references
            $content = $content -replace """$OldName""", """$NewName"""
            $content = $content -replace "'$OldName'", "'$NewName'"
            
            # Replace assembly and namespace names
            $content = $content -replace "<AssemblyName>$OldName</AssemblyName>", "<AssemblyName>$NewName</AssemblyName>"
            $content = $content -replace "<RootNamespace>$OldName</RootNamespace>", "<RootNamespace>$NewName</RootNamespace>"
            
            # Replace all other occurrences with word boundaries
            $content = $content -replace "\b$OldName\.", "$NewName."
            $content = $content -replace "\b$OldName\b", $NewName
            
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
        # Skip binary files or files we can't read
    }
}

# Step 2: Rename all FILES containing the old name (recursively)
Write-Host ""
Write-Host "Step 2: Renaming files with '$OldName' in filename..." -ForegroundColor Cyan
$filesToRename = Get-ChildItem -Path $ProjectPath -Recurse -File | Where-Object { $_.Name -match $OldName }

foreach ($file in $filesToRename) {
    $newFileName = $file.Name -replace $OldName, $NewName
    $newFilePath = Join-Path $file.Directory.FullName $newFileName
    
    if (-not $DryRun) {
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
    } else {
        Write-Host "  [DRY RUN] Would rename: $($file.Name) -> $newFileName" -ForegroundColor Yellow
        $stats.FilesRenamed++
    }
}

# Step 3: Rename all FOLDERS containing the old name (bottom-up)
Write-Host ""
Write-Host "Step 3: Renaming folders with '$OldName' in name..." -ForegroundColor Cyan
$foldersToRename = Get-ChildItem -Path $ProjectPath -Recurse -Directory | Where-Object { $_.Name -match $OldName } | Sort-Object FullName -Descending

foreach ($folder in $foldersToRename) {
    $newFolderName = $folder.Name -replace $OldName, $NewName
    $newFolderPath = Join-Path $folder.Parent.FullName $newFolderName
    
    if (-not $DryRun) {
        if (Test-Path $newFolderPath) {
            Write-Host "  Skipping: $($folder.Name) - target exists" -ForegroundColor Yellow
        } else {
            try {
                Rename-Item -Path $folder.FullName -NewName $newFolderName -ErrorAction Stop
                Write-Host "  Renamed: $($folder.Name) -> $newFolderName" -ForegroundColor Green
                $stats.FoldersRenamed++
            } catch {
                Write-Host "  Error: $($folder.Name) - $_" -ForegroundColor Red
                Write-Host "  Close all programs and try manually" -ForegroundColor Yellow
            }
        }
    } else {
        Write-Host "  [DRY RUN] Would rename: $($folder.FullName) -> $newFolderName" -ForegroundColor Yellow
        $stats.FoldersRenamed++
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
}

Write-Host ""