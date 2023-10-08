# Cotações
Por meio desse projeto você será capaz de monitorar o preço de um ativo da B3 através de um terminal, além de receber alertas sobre a atualização do preço, sugerindo a compra ou a venda de ações.

Para acessar os preços dos ativos, foi utilizada uma [planilha do google sheets](https://docs.google.com/spreadsheets/d/1uypMIKinfs1VS8p2U6y-zzQqqnrPfPkc6V-WTOc1CyI/edit#gid=0) que contém as cotações dos [ativos da B3](https://www.dadosdemercado.com.br/bolsa/acoes). 

## Configuração de envio de email
Antes de iniciar a aplicação, você precisa criar o arquivo de configuração para o envio de email. Para isso, vá até o diretório raiz do projeto e crie um arquivo json chamado *email_config.json*. Ele deverá possuir os atributos abaixo:

```json
{
    "To": "email1@gmail.com",
    "Sender" : "Nome do remetente",
    "From": "email2@gmail.com",
    "Receiver": "Nome do destinatário",
    "Server" : "smtp.gmail.com",
    "Port" : 123,
    "Username" : "user",
    "Password" : "pass"
}
```
## Executando a aplicação
Para executar a aplicação, entre com o nome do ativo a ser monitorado, o preço de referência para venda e o preço de referência para compra. Por exemplo:

```console
dotnet run SNSY6 33.59 33.15
```

No exemplo acima, o usuário quer monitorar o ativo **SNSY6** com o preço de referência para venda de **R$33,59** e o preço de referência para compra de **R$33,15**.

## Limitações
### Atualização de ativos
Como os nomes dos ativos estão contidos em uma planilha, caso haja mais ativos para serem monitorados, ou algum ativo na planilha deixe de ser da B3, a planilha deve ser atualizada manualmente. 

### Cotações indisponíveis
A fórmula [GOOGLEFINANCE](https://support.google.com/docs/answer/3093281?hl=pt-BR) é utilizada para retornar as cotações dos ativos. Porém, a fórmula não consegue retornar as cotações de alguns ativos. Para saber quais são esses ativos, veja a [planilha](https://docs.google.com/spreadsheets/d/1uypMIKinfs1VS8p2U6y-zzQqqnrPfPkc6V-WTOc1CyI/edit#gid=0).