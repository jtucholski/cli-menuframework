# README

This framework is a proof of concept at a lightweight, easy to implement, and configurable console menu framework. 

The following features are explained:


* [Creating a Console menu](#creating-a-console-menu)
* [Menu Option Results](#menu-option-results)
* [Deriving from Console Menu](#deriving-from-console-menu)
* [Menu Defaults](#menu-defaults)
* [Dynamic menus](#dynamic-menus)
* [Using Console Menu Helpers](#using-console-menu-helpers)


# Usage

## Creating a Console Menu

A basic console menu (with options) can be created by instantiating a new `ConsoleMenu`. `AddOption(string, Func<MenuOptionResult>)` is used to add menu options to select from and `Show()` is called when you are ready to display the menu.

> **Note** `Func<MenuOptionResult>` in C# can be read as "provide the name of a parameter-less method that returns a `MenuOptionResult`".

```csharp
public static void Main(string[] args)
{
    ConsoleMenu menu = new ConsoleMenu();
    menu.AddOption("Date and Time", DisplayDateAndTime)
        .AddOption("Weather", DisplayWeather)
        .AddOption("Exit", Exit)
        .Show();
}

private static MenuOptionResult DisplayDateAndTime()
{
    Console.WriteLine($"The current date and time is {DateTime.Now}.");
    return MenuOptionResult.WaitAfterMenuSelection;
}

private static MenuOptionResult DisplayWeather()
{
    Console.WriteLine("Today's weather is a comfortable 60 degrees and sunny.");
    return MenuOptionResult.WaitAfterMenuSelection;
}

private static MenuOptionResult Exit()
{
    Console.WriteLine("Exiting application.");
    return MenuOptionResult.ExitAfterSelection;
}
```

## Menu Option Results

Any method that is called when a menu option is selected, must return a `MenuOptionResult`. The `MenuOptionResult` indicates what will happen next after the code for the option has been executed. `MenuOptionResult` is an `enum` and the choices include:

* `WaitAfterMenuSelection` - The program will pause after the command is executed and the user will need to press any key to continue.
* `DoNotWaitAfterMenuSelection` - The program will NOT pause after the command is executed and the menu options will be re-drawn immediately.
* `WaitThenCloseAfterSelection` - The program will pause after the command is executed before closing the menu containing the option.
* `CloseMenuAfterSelection` - The menu containing the selected option will close after the command is executed.
* `ExitAfterSelection` - The menu, and all parent menus, will close after the command is executed.

## Deriving from Console Menu

You can configure what is shown (and better organize your code) by deriving from the `ConsoleMenu` class. An `OnBeforeShow()` and `OnAfterShow()` method is available for overriding, allowing you further customization of what displays before or after the menu options are rendered.

In this example we create a `MainMenu` with the same options, except now it will display fancy ASCII-art before rendering those options.


```csharp
public class MainMenu : ConsoleMenu
{
    public MainMenu()
    {
        // Add Options to the Main Menu
        this.AddOption("Hello World", HelloWorld)
            .AddOption("Date and Time", DisplayDateAndTime)
            .AddOption("Weather", DisplayWeather)
            .AddOption("Close", Close) // ConsoleMenu.Close
    }

    protected override void OnBeforeShow()
    {
        Console.WriteLine(@" __  __       _         __  __                  ");
        Console.WriteLine(@"|  \/  | __ _(_)_ __   |  \/  | ___ _ __  _   _ ");
        Console.WriteLine(@"| |\/| |/ _` | | '_ \  | |\/| |/ _ \ '_ \| | | |");
        Console.WriteLine(@"| |  | | (_| | | | | | | |  | |  __/ | | | |_| |");
        Console.WriteLine(@"|_|  |_|\__,_|_|_| |_| |_|  |_|\___|_| |_|\__,_|");
        Console.WriteLine();
    }    
}
```


## Menu Defaults

Any `ConsoleMenu`, or derived menu, supports added configuration. You can customize such things as background/foreground colors, selectors, beeps (on error), and whether or not Escape closes the menu.

To configure the menu, call the `Configure(Action<MenuConfig>)` method. This method provides a `MenuConfig` instance for you to get/set each of its public properties.

> **Note** The syntax for `Configure()` mimics syntax similar to what is seen in server-side frameworks like ASP.NET. The name `config` is arbitrary. You will always be given an object of type `MenuConfig`.

```csharp
public class MainMenu : ConsoleMenu
{
    public MainMenu()
    {
        // Add Options to the Main Menu
        this.AddOption("Hello World", HelloWorld)
            .AddOption("Date and Time", DisplayDateAndTime)
            .AddOption("Weather", DisplayWeather)
            .AddOption("Close", Close) // ConsoleMenu.Close
            .Configure(config => {
                config.ItemBackgroundColor = ConsoleColor.White;
                config.ItemForegroundColor = ConsoleColor.Black;
                config.SelectedItemBackgroundColor = ConsoleColor.White;
                config.SelectedItemForegroundColor = ConsoleColor.Black;                
                config.BeepOnError = true;
            });
    }
}
```

### KeyString Mode

One configuration choice you may make, is the decision to use `KeyString` mode for menu selection. Instead of up and down arrow navigation, `KeyString` mode allows the user to enter key strings to select a menu option.

When using KeyString mode, you can supply the value that will be used to select the option as the `keyString` parameter. If you do not supply a value, the options will be auto-numbered.

```csharp
public class MainMenu : ConsoleMenu
{
    public MainMenu()
    {
        // Add Options to the Main Menu
        this.AddOption("Hello World", HelloWorld, "HW")
            .AddOption("Date and Time", DisplayDateAndTime, "DT")
            .AddOption("Display Weather", DisplayWeather, "DW")
            .AddOption("Close", Close, "Q") // ConsoleMenu.Close
            .Configure(config => {
                config.MenuSelectionMode = MenuSelectionMode.KeyString;
                config.ItemBackgroundColor = ConsoleColor.White;
                config.ItemForegroundColor = ConsoleColor.Black;
                config.SelectedItemBackgroundColor = ConsoleColor.White;
                config.SelectedItemForegroundColor = ConsoleColor.Black;                
                config.BeepOnError = true;
            });
    }
}
```

## Dynamic Menus

If your application requires menu options that are dynamically generated from a data source, you can use the `AddOptionRange<T>(IEnumerable<T>, Func<T, MenuOptionResult>)` method provided by `ConsoleMenu`. An example of this syntax follows below in which a `List<Park>` objects is used to build menu options.

```csharp
public class ParksMenu : ConsoleMenu
{
    private List<Park> parks;
    public ParksMenu(List<Park> parks)
    {   
        //1. Indicates the object type that corresponds with the menu option.
        //2. The collection of objects to display as menu options.
        //3. The name of the method that accepts a parameter of type T 
        //   (indicated by 1) and returns a MenuOptionResult.
                            //1   //2    //3
        this.AddOptionRange<Park>(parks, ShowParkDetails)
            .AddOption("Close", Close);
    }

    // The parameter passed in, park, corresponds with the menu option
    // that was selected in the menu.
    private MenuOptionResult ShowParkDetails(Park park)
    {
        Console.WriteLine($"You selected {park.Name}.");
        return MenuOptionResult.WaitAfterMenuSelection;
    }
}
```

### Menu Text for Dynamic Menus

For dynamic menus, by default the `ToString()` method will be called on each object in the data source to determine what text to display on the menu.  If you prefer not to use `ToString()` for this purpose, you may specify a *get text* callback function. This will be called by ConsoleMenu at the time the menu is drawn.  

Also for dynamic menus in *KeyString* menu selection mode, by default each option will be auto-numbered, but you can override this behavior by specifying a *get key string* callback function, to be called when the menu is drawn.

Both of these options options may be included in the call to `AddOptionRange` as follows:

```csharp
    public class ParksMenu : ConsoleMenu
    {
        private List<Park> parks;
        public ParksMenu(List<Park> parks)
        {
            //1. Indicates the object type that corresponds with the menu option.
            //2. The collection of objects to display as menu options.
            //3. The name of the method that accepts a parameter of type T 
            //   (indicated by 1) and returns a MenuOptionResult.
            //4. A callback to a named method that returns the menu text to be displayed
            //5. A callback to an anonymous function that returns the Keystring used to select the option
                                //1   //2    //3              //4          //5
            this.AddOptionRange<Park>(parks, ShowParkDetails, GetMenuText, p => $"{p.ParkId}")
                .AddOption("Close", Close);
        }

        // The parameter passed in, park, corresponds with the menu option
        // that was selected in the menu.
        private MenuOptionResult ShowParkDetails(Park park)
        {
            Console.WriteLine($"You selected {park.Name}.");
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        // This method is called for each park in the list to get the text to be displayed on the menu
        private string GetMenuText(Park park)
        {
            return $"{park.Name.ToUpper()}, {park.State}";
        }
    }
```

### Modifying Menu Options

If your application needs to update the available menu options (i.e. your user can choose an item to delete or change its name), you can override `RebuildMenuOptions()`. `RebuildMenuOptions()` is invoked before the menu options are drawn, allowing your application to display the most up to date options. 

```csharp
protected override void RebuildMenuOptions()
{
    base.ClearOptions();
    this.AddOptionRange<Park>(parks, ShowParkDetails)
        .AddOption("Close", Close);
}
```

## Using Console Menu Helpers

The `ConsoleMenu` class exposes additional user input helper methods. These methods introduce validation and will prompt the user continuously until a valid value is provided. Methods exist for `integer`, `double`, `decimal`, `DateTime`, `bool`, and `string`. They are marked static so that they can be used from anywhere.

```csharp
int age = ConsoleMenu.GetInteger("Enter your age:");
double weight = ConsoleMenu.GetDouble("What is your weight?");
DateTime dob = ConsoleMenu.GetDate("What is your date of birth?");
bool goodDay = ConsoleMenu.GetBool("Are you having a good day?", defaultValue: true);
string name = ConsoleMenu.GetString("What is your name?");
```

Each of the methods supports an optional `defaultValue` allowing you to provide a default value to use if the user presses ENTER.