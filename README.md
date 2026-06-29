# The Cutulu.Godot SDK
Inherting its name from the lovecraftian being Cthulhu, this SDK serves as foundation for software projects powered by godot.

# Dependencies
- Cutulu.Net (https://github.com/Narrenschlag/Cutulu.Net)

### Installation
This repository has to be compiled with your godot project.

**Option A**
1. Download and locate the Cutulu.Net repository
2. Add this to your own project's .csproj file in <Project>
```xml
<ItemGroup>
  <Compile Include="..\Cutulu\**\*.cs">                           <<< Replace ..\Cutulu with the path to your local Cutulu.Net repository
    <Link>..\Shared\%(RecursiveDir)%(Filename)%(Extension)</Link>
  </Compile>
</ItemGroup>
```

**Option B**
Just download Cutulu.Net and add it to the same project.

# Why OpenSource
Sole purpose of this SDK is to improve overall software products and their production cycles by providing optimized and efficient code foundations to almost tailored extend without needing to learn complex algorithms, writing far too many lines of code or producing flaws in the code due to forgetting details.

# Author
My name is Maximilian Schecklmann, also known as Narrenschlag or MaxNar and founder of the Narrenschlag Collective. My passion and profession is creating software, solving problems, creating art and enhancing the everyday experience of people. My coding journey started in game development and it's this heritage that I base all my interest and experience on till this day. As my personal ambition is to become the **Rodin** of my generation, I want to build my legacy by envoking emotion in people through art and high quality. By supporting this SDK you support my dream and as you support me you simultaneously support this project. Therefore. Thank you.

~ *Max*



# Development Support
As this is my personal library I will continue to develop this SDK for the newest versions of Godot with support for previous Godot 4.X versions. So do not worry about that. Further you can use most of the code for .Net and .Net Core independently as it's written mostly without Godot's utility. Feel free to take code snippets and fork this library but please do not forget the credit as it helps me on my personal mission.



## License

MIT © Narrenschlag
