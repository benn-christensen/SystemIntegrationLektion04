# SystemIntegration Lektion 4

## Opgave 1: Publish/Subscribe

Følg guiden [RabbitMQ toturial - Publish/Subscribe](https://www.rabbitmq.com/tutorials/tutorial-three-dotnet) for at implementere
en Publish/Subscribe model ved hjælp af RabbitMQ i .NET.

## Opgave 2: Sønderhøj Taxi

Lav et web-api med et enkelt endpoint, hvor man kan bestille en taxi. I bestemmer selv hvilke 
informationer der skal sendes med i bestillingen. Når api'et modtager en bestilling, 
skal denne bestilling sendes videre til de taxaer der er tilmeldt systemet. 

Taxa applikationen skal laves som en konsol applikation, hvor I udskriver til konsollen når der 
modtages en bestilling. I skal bruge RabbitMQ til at sende bestillingen fra web-api'et til taxa applikationen.


