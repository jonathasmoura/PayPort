# PayPort
Este projeto foi desenvolvido com foco em demonstrar uma implementação próxima de um cenário real encontrado em instituições financeiras, aplicando padrões arquiteturais modernos, boas práticas de engenharia de software e tecnologias amplamente utilizadas no mercado para construção de aplicações robustas, escaláveis e de fácil manutenção


## 📋 Sobre o projeto

Esta aplicação simula um ambiente corporativo utilizado por instituições financeiras para o processamento de notificações de pagamentos (Webhooks) enviadas por bancos parceiros.

O sistema é responsável por receber confirmações de liquidação de seguros e parcelas de empréstimos, validar a autenticidade da requisição, impedir o processamento duplicado (Idempotência), persistir as informações e disponibilizar o status das operações através de um painel administrativo.

O objetivo deste projeto é demonstrar uma arquitetura escalável, desacoplada e preparada para ambientes de alta disponibilidade, seguindo boas práticas de desenvolvimento utilizadas em sistemas críticos do mercado financeiro.

## 🎯 Cenário

Um banco parceiro envia Webhooks sempre que ocorre a liquidação de um pagamento.

Cada evento recebido deve ser processado com segurança, garantindo que:

- O mesmo evento nunca seja processado duas vezes.
- Os dados sejam persistidos de forma consistente.
- O histórico de processamento fique disponível.
- O status do pagamento possa ser consultado em tempo real.
- Falhas de processamento possam ser identificadas facilmente.
- O sistema esteja preparado para crescimento e integração com novos parceiros.

## 🔒 Regras de negócio

- Cada Webhook possui um identificador único.
- Eventos repetidos não devem ser processados novamente.
- O histórico de processamento deve permanecer armazenado.
- Toda alteração de status deve ser auditada.
- Apenas eventos válidos são persistidos.
- Pagamentos podem possuir diferentes status de processamento.
- O painel administrativo deve refletir o estado mais recente da operação.

## 📈 Objetivos técnicos

Este projeto tem como foco demonstrar conhecimentos em:

- Arquitetura em camadas
- Princípios SOLID
- Domain Driven Design
- Clean Architecture
- APIs REST
- Processamento de Webhooks
- Idempotência
- Validação de regras de negócio
- Tratamento de exceções
- Boas práticas de organização de código
- Separação de responsabilidades
- Desenvolvimento Full Stack utilizando .NET e React
