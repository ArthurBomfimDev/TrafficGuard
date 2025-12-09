<div align="center">
<img src="https://placehold.co/1200x300/ffff00/000000?text=TrafficGuard&font=raleway" alt="Banner do Projeto TrafficGuard">
</div>

<h1 align="center">🚦 TrafficGuard: Sistema Distribuído de Ingestão e Processamento de Infrações</h1>

<p align="center">
  <img src="https://img.shields.io/badge/.NET%2010.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 10">
  <img src="https://img.shields.io/badge/Python-3776AB?style=for-the-badge&logo=python&logoColor=white" alt="Python">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white" alt="C#">
  <img src="https://img.shields.io/badge/Entity%20Framework%20Core-512BD4?style=for-the-badge&logo=nuget&logoColor=white" alt="Entity Framework Core">
  <img src="https://img.shields.io/badge/PostgreSQL-336791?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL">
  <img src="https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white" alt="RabbitMQ">
  <img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" alt="Docker">
  <img src="https://img.shields.io/badge/Architecture-Clean%20%26%20DDD-blue?style=for-the-badge" alt="Clean Architecture & DDD">
  <img src="https://img.shields.io/badge/MediatR-CQRS-2E86C1?style=for-the-badge&logo=mediatr" alt="MediatR & CQRS">
  <img src="https://img.shields.io/badge/QuestPDF-PDF%20Generation-red?style=for-the-badge" alt="QuestPDF">
</p>


```mermaid
  graph TB
      subgraph "Presentation Layer"
          API[API REST .NET 10]
          GRPC[gRPC Services]
          SignalR[SignalR Hub]
      end
      
      subgraph "Application Layer"
          CQRS[CQRS Pattern]
          MediatR[MediatR Pipeline]
          Valid[FluentValidation]
      end
      
      subgraph "Domain Layer"
          Core[Core Entities]
          Agg[Aggregates]
          DomEv[Domain Events]
          Spec[Specifications]
      end
      
      subgraph "Infrastructure Layer"
          EF[Entity Framework]
          RedisC[Redis Cache]
          Hang[Hangfire Jobs]
          Ext[External Services]
      end
      
      API --> CQRS
      GRPC --> CQRS
      CQRS --> Core
      MediatR --> CQRS
      Core --> EF
      EF --> RedisC
      DomEv --> Hang
      Ext --> SignalR
```


<table> <thead> <tr> <th>Categoria</th> <th>Pacote</th> <th>Versão</th> <th>Propósito</th> </tr> </thead> <tbody> <tr> <td><strong>Arquitetura</strong></td> <td>MediatR</td> <td>12.2.0</td> <td>Padrão Mediator/CQRS</td> </tr> <tr> <td><strong>Validação</strong></td> <td>FluentValidation</td> <td>11.8.0</td> <td>Validação de comandos e DTOs</td> </tr> <tr> <td><strong>Mapeamento</strong></td> <td>AutoMapper</td> <td>12.0.1</td> <td>Transformação entre objetos</td> </tr> <tr> <td><strong>Documentação</strong></td> <td>Swashbuckle.AspNetCore</td> <td>6.6.1</td> <td>Documentação Swagger/OpenAPI</td> </tr> <tr> <td><strong>Processamento</strong></td> <td>Tesseract</td> <td>5.2.0</td> <td>OCR para leitura de placas</td> </tr> <tr> <td><strong>Imagens</strong></td> <td>OpenCvSharp</td> <td>4.9.0</td> <td>Processamento de imagens</td> </tr> <tr> <td><strong>Jobs</strong></td> <td>Hangfire</td> <td>1.8.6</td> <td>Processamento em background</td> </tr> <tr> <td><strong>Monitoramento</strong></td> <td>Serilog</td> <td>3.1.1</td> <td>Logs estruturados</td> </tr> </tbody> </table>
