# GamePlatform: A Platform for all Games
## Este es un repositorio hecho en Visual Studio 2022, consta de dos proyectos ejecutables (Windows Form):
 
 - Domino.Net (principal)
 - StarCraft

Desde el directorio de la solución del proyecto ejecutar en la consola:

- dotnet run --project Domino.Net
- dotnet run --project StarCraft

## Consta de dos bibliotecas de clases (Net Core 6.0):
 
- DominoPlatform (principal)
- StarCraftPlatform
 
 # Domino
Tiene la posibilidad de escoger como quiere que funcione el juego, los cambios que realice serán controlados para que sean válidos. 
Cuando se presiona el botón ` Start Game `, si hay la cantidad de jugadores necesarios, se inicializa el juego.

![settings](https://user-images.githubusercontent.com/96163553/179070482-c8eb3529-2852-4571-bf4a-1f74fd944e06.png)



- __Los jugadores__: First Player (juega lo primero que ve); Random Player; Bota Gorda; Bota Suave; Smart Player.
- __El tipo de ficha__: Ficha clásica de números; Ficha múltiplo de tres(la suma de los lados juntados tiene que ser múltiplo de tres);  Ficha clásica de color.
- __Condiciones de finalización__(es obligada la condición de trancarse): condición de Pegarse y con cuantas fichas se considera pegado el jugador; condición de terminar el juego si alguien se pega.
- __Robar__: robar fichas de afuera cuando no se tiene una jugada valida
- __Plin__: Si el jugador pone el cinco, pasa al que le toca el turno.
- __Generate__: Generar todas las fichas o una cantidad menor al azar.
- __Distribute__: Random o en orden.
- __PassTurn__: Pase de turno clásico o se invierte el orden si se pone una ficha con todos sus lados iguales.
- __Winner__: Gana el que menos puntos tenga al acabar el juego(clásico) o el que tiene la mayor cantidad de lados iguales.
- __Initial Hand__: Mano inicial del jugador.
- __Top__: Número (o cantidad de colores) tope en las fichas.
- __Sides__: Cantidad de lados de la ficha.
- __Plays by sides__: Jugadas válidas por cada cara de la ficha.

# StarCraft
![StarCraft 2](https://user-images.githubusercontent.com/96163553/179073770-21e0cd48-7519-4f1c-b791-6e1f25cbc4cd.png)

## Instrucciones
- Arrows(➡,⬅,⬆,⬇): Moverse.
- P : Pasar el turno.
- A : Atacar.
- Numpad # : Tomar el artículo #.



