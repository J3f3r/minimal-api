# Minimal API

**Projeto:** Minimal API  
**Autor:** Jeferson Martins com tutorial da DIO em Bootcamp em .NET 
**Objetivo:** Projeto de estudo/prática em .NET (C#) para aprender a montar uma API leve, com testes automatizados, migrações e preparo para deploy. Feito por um estudante de TI que está se preparando para vagas Jr em backend.

---

## 🚀 Visão geral
Este repositório contém uma API minimalista construída em .NET (C#) com:

- projeto `API/` — código da aplicação (.NET, Program.cs, Startup.cs, Migrations, Infraestrutura, Domínios);
- projeto `Test/` — testes automatizados (mocks, requests, helpers);
- pasta `publish/` — conteúdo gerado para deploy (contém assets/zip usados em tentativas de deploy);
- dump SQL (`minimal_api.dump.sql`) para popular o banco (quando necessário).

O foco foi: estruturar projeto, aplicar migrações, criar testes e entender o fluxo de build → publish → deploy (deploy para AWS ficou incompleto por falta de créditos/experiência).

---

## 🧰 Tecnologias usadas
- .NET (C#) — ASP.NET Core (Minimal API / startup tradicional conforme o código)
- Entity Framework Core (migrations presentes)
- Projeto de testes com mocks
- SQL (dump disponível)
- Ferramentas: `dotnet` CLI, VS Code / Visual Studio

---
## ✅ O que aprendi com este projeto
- Estruturar uma API em .NET.
- Criar e aplicar migrations com EF Core.
- Rodar e validar testes unitários.
- Preparar artefatos para publish/deploy.
- Primeiros passos no deploy em nuvem (AWS).

## Deploy na AWS Elastic Beanstalk
A tentativa de deploy não foi concluída por:
- Falta de créditos na AWS
- Dificuldade com configuração (IAM, variáveis, health checks)

## ✍️ Como colaborar
- Fork o repositório.
- Crie uma branch: git checkout -b feat/nome-da-feature.
- Commit → Push → Pull Request.

## REFERÊNCIAS: DIO
