FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /app

COPY . .

# Install jq for JSON parsing
RUN apt-get update && apt-get install -y jq

# Parse configuration
RUN PROJECT_NAME=$(jq -r '.projectName' build-config.json) && \
    DOTNET_VERSION=$(jq -r '.dotnetVersion' build-config.json) && \
    PACKAGE_NAME=$(jq -r '.packageName' build-config.json) && \
    PACKAGE_VERSION=$(jq -r '.packageVersion' build-config.json) && \
    BUILD_CONFIGURATION=$(jq -r '.buildConfiguration' build-config.json) && \
    echo "PROJECT_NAME=$PROJECT_NAME" >> /etc/environment && \
    echo "DOTNET_VERSION=$DOTNET_VERSION" >> /etc/environment && \
    echo "PACKAGE_NAME=$PACKAGE_NAME" >> /etc/environment && \
    echo "PACKAGE_VERSION=$PACKAGE_VERSION" >> /etc/environment && \
    echo "BUILD_CONFIGURATION=$BUILD_CONFIGURATION" >> /etc/environment

# Install .NET
RUN curl https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh -o ./dotnet_script.sh && \
    bash ./dotnet_script.sh --channel $(jq -r '.dotnetVersion' build-config.json)

ENV PATH="${PATH}:/root/.dotnet"

# Build project
RUN . /etc/environment && \
    dotnet new classlib -n $PROJECT_NAME && \
    cd $PROJECT_NAME && \
    dotnet add package $PACKAGE_NAME --version $PACKAGE_VERSION && \
    cp ../$PROJECT_NAME.cs . && \
    dotnet build --configuration $BUILD_CONFIGURATION

# Copy the build output to a known location
RUN . /etc/environment && \
    mkdir /build_output && \
    cp -r $PROJECT_NAME/bin/$BUILD_CONFIGURATION/* /build_output/

ENTRYPOINT ["tail", "-f", "/dev/null"]
