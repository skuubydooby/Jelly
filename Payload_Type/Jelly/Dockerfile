FROM mcr.microsoft.com/dotnet/sdk:8.0

# Install system dependencies
RUN apt-get update && apt-get install -y \
    python3 \
    python3-pip \
    python3.11-venv \
    wget \
    unzip \
    && rm -rf /var/lib/apt/lists/*

# Build and install donut
RUN curl -L -o donut_shellcode-2.0.0.tar.gz https://github.com/MEhrn00/donut/releases/download/v2.0.0/donut_shellcode-2.0.0.tar.gz && \
    tar -xf donut_shellcode-2.0.0.tar.gz && \
    cd donut_shellcode-2.0.0 && \
    make && \
    cp donut / && \
    cd .. && \
    rm -rf donut_shellcode-2.0.0 && \
    rm -rf donut_shellcode-2.0.0.tar.gz

# Create Python virtual environment (at root level to survive volume mounts)
RUN python3 -m venv /venv
RUN /venv/bin/python -m pip install --no-cache-dir mythic-container==0.6.6 mslex impacket toml
RUN /venv/bin/python -m pip install --no-cache-dir git+https://github.com/MEhrn00/donut.git@v2.0.0

# Set working directory
WORKDIR /Mythic/

# Copy project files
COPY [".", "."]

# Download System.Management.Automation DLL for PowerShellHost
RUN mkdir -p /Mythic/Jelly/agent_code/packages/System.Management.Automation6.1.7 && \
    cd /Mythic/Jelly/agent_code/packages/System.Management.Automation6.1.7 && \
    wget https://www.nuget.org/api/v2/package/System.Management.Automation/6.1.7 -O package.zip && \
    unzip -q package.zip && \
    cp lib/netstandard2.0/System.Management.Automation.dll . && \
    rm package.zip && \
    cd /Mythic/Jelly/agent_code

# Restore .NET dependencies and setup donut
RUN cd Jelly/agent_code && \
    dotnet restore && \
    rm -f donut && \
    cp /donut donut

# Copy COFFLoader.dll to root
RUN cd Jelly/agent_code && cp COFFLoader.dll /COFFLoader.dll

# Runtime command - copy donut to mounted volume and start Python service
CMD ["bash", "-c", "cp /donut Jelly/agent_code/donut && /venv/bin/python main.py"]