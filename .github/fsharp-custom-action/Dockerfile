FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /proj

COPY src/ .

# Restore + build + publish the project
RUN dotnet publish -c Release -o out

# ========================================================
# Runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0

ENV VALUES_TO_TAKE="-1"
ENV WORKFLOW_FILE_NAME=""
ENV WORKFLOW_KEY=""

WORKDIR /proj

RUN apt update && apt install -y git yq

COPY --from=build /proj/out .
COPY --chmod=0755 run.sh .

ENTRYPOINT ["run.sh"]