# Battleships Game with a Web Application and Console Application
### This was a mandatory project in TalTech university course ICD0008 Programming in C#
This was my first ever medium-large project in C#

The project uses MVC pattern, a Database connection to store games, a Menu System for navigation inside the Game menu.

When developing this game, some key principles I keep in mind:
* Game Logic (Business Logic) and User Interface must be kept separate!
* Game Logic must not contain code that is not related to the game!
* A nice UI is what makes the difference

# Usage

To play this game, a database connection is required.

The database Model can be created according to the Models in the Domain project.

This project uses Entity Framework Core as the ORM system.

* The WebApp database connection is created in appsettings.json
* The Console App Database connection is created in Domain project BattleShipsDb class.

Run either the WebApp or CSharp project to play the game.
