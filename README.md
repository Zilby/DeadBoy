Google Docs: https://drive.google.com/drive/folders/1VqfBMsc2E3Q5fMgyBK8CIJ2HkOHcTpgA

# Code Bible

## Folders

All scripts must go in the Assets/Scripts folder. Editor scripts must go in the Assets/Scripts/Editor folder. 

## Conventions

You can use whatever code conventions you're most comfortable with. That being said, here's how I'll be formatting my code, and will auto-format your code should changes need to be made (this can be done in your editor settings): 

All code uses camelCase with the exception of const variables. Consts are formatted in all-caps using underscores, as such: 
`const int THIS_IS_A_CONST = 5;`

Function and class names start with uppercase (`ExampleFunctionName`), variable names start with lowercase (`exampleVariableName`).

All code should be indented appropriately (most editors have an auto-indent function, that will work fine). Indents are tabs, not spaces. 

Functions are spaced one new line apart. 

Variables should be defined in the order of public, protected, and then private. (Getters/Setters defined after private in the same order). 

Functions have no space before their parens (ie: `Function()` not `Function ()`). if, for, and while statements do (ie: `if (bool)` not `if(bool)`). 

All brackets (ie: {} ) get their own line.

Remember: you're allowed to break the conventions, just don't be upset when your file gets edited and the conventions are forced upon you. Also I will make fun of you if you name your functions like `Example_function()`. 

## Commenting

All functions and variables need a \<summary\> comment. Summary comments in Unity allow us to hover over text and see the comment in the summary, even in another file. Summary comments look like this:

```
/// <summary>
/// Comment goes here. 
/// </summary>
```

Hitting `///` above a function/variable in Unity's Visual Studio will auto-generate a summary comment that you can fill in. 

In-line comments (eg: `// comment`) are fine, but I'd only recommend them if your code is doing something that's difficult to understand at first glance. 

Creating a super complicated long function that's peppered with in-line comments is NOT allowed, that just indicates your code is bad. Divide up the function into smaller parts, and add summary comments to those individual functions. 

## Attributes

Attributes are vastly underutilized by newer Unity programmers, but they make scene editing a LOT easier. 

The easiest attribute to understand is `[Header("Example Grouping")]`. Headers group public variables together under a header in the inspector, making them easier to read/understand. In most cases, you should use headers to group public variables if there are more than 2 or 3. 

Another useful attribute for grouping is `[Tooltip("Comment")]`. Tooltips are the text that will show up in the inspector when you hover over a variable, useful if a user hovers over a variable with a name that is not immediately obvious. 

`[Range(0, 10)]` forces variable to only be a range of numbers, creating a slider in the inspector. 

Finally the custom `[ConditionalHideAttribute("Bool", hideInInspector, inverse, 0, 1)]` will hide a variable in the inspector based on the given "Bool" (name of a bool or getter of a bool in the monobehaviour). hideInInspector determines whether or not the variable is uninteractable or hidden. Inverse will determine whether the variable is hidden if the bool is true or false. Finally the last two digits will allow you to enter a range of numbers since `ConditionalHideAttribute` doesn't play nice with `Range`. 

Also worth noting: any variables that are public that do not need to be accessed in the inspector should have either the `[HideInInspector]` or `[System.NonSerialized]` attributes attached (or simply make the variable private and add a public getter/setter). 

## Event-based Programming

Tired of having references to every object in a scene? Wish you could access non-static variables in static functions? Well boy howdy do I have the answer to both of those highly specific inquiries. 

Event based programming can be summed up as the usage of these lines of code:

```
using System;

public static Action<int> ActionExample;

public static Func<int, bool> FuncExample;

public delegate int DelegateExample(string s = "example");
public static DelegateExample DelegateEvent;
```

So what are these things exactly? Delegates are function placeholders that can be assigned to. What's so great about delegates is that a static delegate can have references to multiple non-static functions used by different monobehaviours. As an example, want to tell every single character in the scene to jump without a reference to them? You can't with a static function, since every character's transform is non-static, but you can with a delegate that has been assigned the jump function of every character. 

So what are Actions and Funcs then? These System types are essentially just shorthand for delegates. As a quick tutorial:
```
Action A == delegate void D()
Action<T> A == delegate void D(T t)
Func<T> == delegate T D()
Func<T1, T2> == delegate T2 D(T1 t) 
// worth noting, you can add as many 'T's as you like, the return type will always be the last one. 
```
The only time you legitimately need to use the delegate longform is if you want to assign defaults for values, as seen in the first example. 

So how do you assign to delegates? It's really quite simple. 
```
public static Func<int, bool> FuncExample;
public bool FunctionExample(int i) { return true; }

void Awake() 
{
  FuncExample += FunctionExample;
}
```
`+=` will add the function to the delegate (the delegate will call all functions that are added), while `=` will assign the delegate to only that function. Worth noting however is to ensure that you remove the function from the delegate if it's going to be used after that object is destroyed (including when a new scene is loaded), otherwise you'll get a null reference to the function. Thankfully this is quite easy to do:
```
void OnDestroy() 
{
  FuncExample -= FunctionExample;
}
```

## UI and Inspector Conventions

#### Text and TextMeshPRo
First and foremost, we will not be using default Unity UI text (ie: NEVER create a 'Text' component). Default Unity text is awful and honestly should never be used in a professional, or even hobbyist program. Instead we will be using TextMeshPro (TMPro) text (specifically `TextMeshPro - Text(UI)` when adding a component in the inspector, and `TextMeshProUGUI` in the code). TextMeshPro vastly improves what we can do with text and to what degree we can customize it. 

Since we will be using TMPro text, we will also be using their other UI components when possible, such as their dropdown and inputfield components. 

#### Unity OnClick Events
We will NOT be assigning ANY OnClick events in the inspector in this project. Having functions called outside of the code makes them difficult to find and debug, and leads to sloppy implementations. Instead all OnClick Events will be assigned in the code, eg:

```
public Button b;
public Button b2;

void Function1() {}
IEnumerator Function2() {}

void Start 
{
  b.OnClick.AddListener(Function1); // you don't need to wrap in a delegate if the function is void without params
  b2.OnClick.AddListener(delegate { StartCoroutine(Function2); });
}
```
