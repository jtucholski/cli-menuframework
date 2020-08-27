# README

This framework is a proof of concept at a lightweight, easy to implement, console menu framework. 

The following features are explained:

* [Creating a static menu](#creating-a-static-menu)
* [Passing data to menu commands](#passing-data)
* [Dynamic menus](#dynamic-menus)
* [Menu Configuration](#menu-configuration)


Two key decisions were made putting the framework together that influenced the design:
* A fluent interface implemented using method chaining.
* The usage of delegates to associate commands with each menu option.

Both of these concepts are unfamiliar to the beginner programmer but are often used in modern server-side frameworks.

## Usage

### Creating a static menu

This example demonstrates the simplest use case for creating a static menu.

![Basic Usage Example](gifs/menu-basic.gif)

**Code**

```csharp
static void Main(string[] args)
{    
    ConsoleMenu mainMenu = new ConsoleMenu()
        .AddOption("Greeting", ShowGreeting)
        .AddOption("Current Time", CurrentTime)
        .AddOption("Close", ConsoleMenu.Close);
    
    mainMenu.Show();
}

static void ShowGreeting()
{
    Console.WriteLine("Hello There!");
}

static void CurrentTime()
{
    Console.WriteLine($"The current date and time is {DateTime.Now}");
}
```

### Passing Data

If your command requires data for context, `AddOption<T>` supports providing an item to pass onto the callback action.


```csharp
static void Main(string[] args)
{         
    Park park1 = new Park() { Name = "Cuyahoga Valley National Park", Abbrebiation = "CVNP", State = "OH" };
    Park park2 = new Park() { Name = "Grand Canyon National Park", Abbrebiation = "GCNP", State = "AZ" };
    Park park3 = new Park() { Name = "Acadia National Park", Abbrebiation = "ANP", State = "ME" };

    ConsoleMenu mainMenu = new ConsoleMenu()
        .AddOption<Park>("CVNP", park1, ParkSelection)
        .AddOption<Park>("GCNP", park2, ParkSelection)
        .AddOption<Park>("ANP", park3, ParkSelection)
        .AddOption("Close", ConsoleMenu.Close);
        
    mainMenu.Show();
}

static void ParkSelection(Park selection)
{
    // i.e. "You selected Cuyahoga Valley National Park.
    Console.WriteLine($"You selected {selection.Name}.");
}
```

### Dynamic Menus

Dynamic menus are supported by calling `AddOptionRange<T>` where `T` is the type of the collection passed in. To control how the menu text is displayed, override `.ToString()`.


```csharp
static void Main(string[] args)
{    
    List<Park> parks = new List<Park>()
    {
        new Park() { Name = "Cuyahoga Valley National Park", Abbrebiation = "CVNP", State = "OH" },
        new Park() { Name = "Grand Canyon National Park", Abbrebiation = "GCNP", State = "AZ" },
        new Park() { Name = "Acadia National Park", Abbrebiation = "ANP", State = "ME" }
    };  

    ConsoleMenu mainMenu = new ConsoleMenu()
        .AddOptionRange<Park>(parks, ParkSelection)
        .AddOption("Close", ConsoleMenu.Close);
        
    mainMenu.Show();
}

static void ParkSelection(Park selection)
{
    // i.e. "You selected Cuyahoga Valley National Park."
    Console.WriteLine($"You selected {selection.Name}."); 
}
}
```


### Menu Configuration

Each menu supports configuration with background and foreground colors, selectors, titles, and more.

The `.Configure()` method accepts a single argument, `MenuConfig`, that is used to modify the default configuration for each menu.

![Menu Configuration](gifs/menu-configuration.gif)


```csharp
static void Main(string[] args)
{
    ConsoleMenu mainMenu = new ConsoleMenu()
        .AddOptionRange<Park>(parks, ParkSelection)
        .AddOption("Close", ConsoleMenu.Close)
        .Configure(config => { 
            config.SelectedItemBackgroundColor = ConsoleColor.Red;
            config.SelectedItemForegroundColor = ConsoleColor.White;
            config.Selector = "* ";
            config.Title = "PARK SELECTION";
        });

    mainMenu.Show();
}
```

