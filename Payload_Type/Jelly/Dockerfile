FROM mcr.microsoft.com/dotnet/sdk:8.0

# Install system dependencies
RUN apt-get update && apt-get install -y \
    python3 \
    python3-pip \
    python3.11-venv \
    wget \
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

# Create Python virtual environment
RUN python3 -m venv /venv
RUN /venv/bin/python -m pip install --no-cache-dir mythic-container==0.6.6 mslex impacket toml
RUN /venv/bin/python -m pip install --no-cache-dir git+https://github.com/MEhrn00/donut.git@v2.0.0

# Set working directory
WORKDIR /Mythic/

# Copy project files
COPY [".", "."]

# Download System.Management.Automation DLL from Apollo's repo
RUN mkdir -p /Mythic/Jelly/agent_code/packages/System.Management.Automation6.1.7 && \
    wget -O /Mythic/Jelly/agent_code/packages/System.Management.Automation6.1.7/System.Management.Automation.dll \
    https://raw.githubusercontent.com/MythicAgents/Apollo/master/Payload_Type/apollo/apollo/agent_code/packages/System.Management.Automation6.1.7/System.Management.Automation.dll

# Restore .NET dependencies
RUN cd Jelly/agent_code && \
    dotnet restore && \
    rm -f donut && \
    cp /donut donut

# Copy COFFLoader.dll to root
RUN cd Jelly/agent_code && cp COFFLoader.dll /COFFLoader.dll

# Runtime command
CMD ["bash", "-c", "cp /donut Jelly/agent_code/donut && /venv/bin/python main.py"]