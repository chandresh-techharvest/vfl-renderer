# Technical Design for .NET 9 Sitefinity Renderer

## Overview
This document outlines the technical design for the .NET 9 Sitefinity Renderer web app and the OSApi library.

## Architecture
- **Web App**: VFL.Renderer
- **OSApi Library**: VFL.Renderer.OSApi

### Module Breakdown
1. Rendering Module
2. API Client Module
3. Authentication Module
4. Logging Module

## Configuration
### ApiSettings
- API base URL
- Authentication tokens

### Sitefinity
- Sitefinity project configuration settings

## Authentication Schemes
- **Cookie Authentication**: Default scheme for user sessions.
- **MyBillAuth**: Alternate authentication for MyBill users.

## Middleware Flow
- Authentication middleware
- Logging middleware
- Exception handling middleware

## Build/Publish Pipeline
- Managed via Azure Pipelines. Configuration in `azure-pipelines.yml` and `azure-pipelines-1.yml`.

## Logging
- Integrated Serilog for logging.
- Logs written to MSSqlServer.

## Security Considerations
- Protect sensitive data.
- Regular security assessments.

## Deployment
- Deployed via Azure services. Continuously integrated and deployed using the Azure pipeline.