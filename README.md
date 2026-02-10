# 📈 Monitor de Ativos B3 (Desafio Inoa)

Uma aplicação de console em C# que monitora cotações de ativos da B3 em tempo real e envia alertas automáticos por e-mail quando os preços atingem alvos definidos de compra ou venda.

## 🚀 Funcionalidades

- Consulta dados da API da Brapi em tempo real.
- Monitoramento contínuo em loop.
- **Alerta de Compra:** Dispara e-mail quando o preço cai abaixo do limite definido.
- **Alerta de Venda:** Dispara e-mail quando o preço sobe acima do limite definido.
- Configuração segura de credenciais via arquivo JSON externo.

## 🛠️ Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) (Versão 6.0 ou superior).
- Uma conta de e-mail (Gmail recomendado) com "Senha de App" configurada.

## 📝 Sobre o Desenvolvimento

Para a realização deste desafio técnico, adotei uma abordagem de aprendizado prático, utilizando ferramentas de IA como mentoria para acelerar minha transição de linguagens que já dominava (C e Python) para o C#.

O desenvolvimento seguiu uma estrutura cronológica de estudos:
1.  Iniciei pela compreensão da sintaxe e tipagem do C#.
2.  Avancei para a modelagem de classes e serialização JSON.
3.  Implementei a comunicação assíncrona com a API da Brapi.
4.  Finalizei com o sistema de disparo de e-mails via SMTP.

Todo o código foi construído e testado localmente no VS Code, garantindo o funcionamento pleno da lógica antes da publicação. A etapa final do processo consistiu no estudo de versionamento via YouTube, onde aprendi a configurar o Git e o GitHub para realizar o envio do projeto completo, consolidando todo o trabalho desenvolvido em um único commit de entrega.
