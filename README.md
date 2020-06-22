# Microsoft Graph Presence Monitor

Aplicação console dotnet core que consome a API [Microsoft Graph](https://docs.microsoft.com/en-us/graph/overview) para pegar o status de presença do usuário no Microsoft Teams.

## Motivação
A proposta deste projeto é conseguir capturar o status do Microsoft Teams, enviar para um placa controlando uma matriz de leds RGB 	para refletir o status fora do ambiente em que estiver trabalhando. Este projeto pode ser utilizado para exemplo de como obter o token de acesso, mas funcionará mesmo em conjunto com este outro projeto [LedMatrixRestAPI](https://github.com/feokuma/LedMatrixRestAPI) que recebe comando via RestAPI para acionar uma matriz de leds com cor referênte ao status. 

## Pré-requisitos
### Registrar um aplicativo no portal azure
Para conseguir consumir a API do Microsoft Graph será necessário registrar uma aplicação no [Portal Azure](http://portal.azure.com) para ter o TenantId e o ClientId. Esta aplicação deverá ter as seguinte características:

- **Suported account types**: Any Azure AD Directory - Multitenant)
- **Redirect URi**: Public client/native - http://localhost
- **API Permissions**: Microsoft Graph - Delegated Permissions
	- offline_access
	- Presence.Read


### Hardware para apresentação do status
- Implementar o projeto [LedMatrixRestAPI](https://github.com/feokuma/LedMatrixRestAPI) ou criar um servidor Rest que o endpoint para onde os status são enviados

## Dependências
- .net Core 3.x
- Newtonsoft
- Microsoft.Identity.Client
- Flurl e Flurl.Http


## Rodando a aplicação
Para executar a aplicação, é importante que o servidor Rest API do projeto [LedMatrixRestAPI](https://github.com/feokuma/LedMatrixRestAPI) já esteja em execução. 
No código, substitua nas variáveis de tenandId e ClientId com as chaves da aplicação que foi registrada no portal azure e execute a aplicacáo com o seguinte comando

```
dotnet run
```
Assim que a aplicação abrirá o browser para que o usuário possa fazer o login na conta microsoft. Lembrando que esta conta deve estar debaixo do mesmo Tenant que da conta usada para Registrar a Aplicação. O processo de autenticação pedirá para o usuário dar as permissões para acesso a algumas informações da conta.
Sempre que o status no Presence tiver alguma alteração, será apresentado no terminal um log do novo status capturado pelo aplicação. Algo deste tipo será apresentado nos logs.

```json
{
  "@odata.context": "https://graph.microsoft.com/beta/$metadata#users('5bad7daa-74c2-440b-bc1a-6ed121cbef45')/presence/$entity",
  "Id": "5bad7daa-74c2-440b-bc1a-6ed121cbef45",
  "Availability": "Away",
  "Activity": "Away"
}
```

O token expira a cada hora e, sempre que a aplicação precisa requisitar um novo token, será apresentado no terminal um log para informar que o token foi atualizado. 

```
TOKEN REFRESHED
```