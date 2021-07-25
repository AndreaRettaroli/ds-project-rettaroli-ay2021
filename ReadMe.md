## ENDPOINT

https://7ge1zdatk1.execute-api.eu-west-1.amazonaws.com/Prod

## SWAGGER 

https://7ge1zdatk1.execute-api.eu-west-1.amazonaws.com/Prod/swagger/index.html

## Accorgimenti

Le password necessitano di: 

* Lunghezza minima 8 caratteri
* Almeno un numero
* Almeno un carattere speciale
* Almeno una lettera maiuscola
* Almeno una lettera minuscola

## Guida all'uso tramite swagger

1. Registrare un utente, POST api/Users inserendo una mail valida. Se non si desidera intasare la propria casella di posta si consiglia di utilizzare [mailinator ](https://www.mailinator.com/) . Inseriti i dati si prema execute, se tutto è andato a buon fine dovreste vedervi arrivare una mail di benvenuto con la password temporanea-

2. Si effettui il signIn con i dati presenti nella mail. Si otterrà in risposta l'utente con annesso token, si faccia attenzione a selezionare solo il token che ha come nome token, si copi il campo e si vada su Authorize (in alto a destra), si scriva "Bearer token_value" esempio "Bearer eyJraWQiOiI0.....Md1cPRfgbgIR7t7g" .

3. Ora è possibile effettuare tutte le chiamate.  

   

   

   

   