# JumpDiveClock

A speedrun timer for X11. Intended for my personal use. I add features that seem useful to me.

## Installation

### Dependencies

* xinput.

## FAQ

### Why?

I tried some timers for Linux and I either disliked them or they didn't work at all. So I made my
own. And made this to share to other people that may find it useful.

### What features do you have?

* marking time (duh);
* comparing the current time with your personal best;


### Where did this name come from?

I speedrun a Mario fangame where the fastest way to move without equipment is to repeatedly jump and
dive.

## Running

Ensure you have
[Dotnet](https://learn.microsoft.com/en-us/dotnet/core/install/linux?WT.mc_id=dotnet-35129-website)
installed.

First, navigate to the folder where you want to install it using `cd`. And then, run the following
commands:

```sh
git clone https://github.com/NickatelbNornkirn/JumpDiveClock.git
cd JumpDiveClock
dotnet run --configuration Release
```

## Usage

You are free to modify and/or redistribute copies of this software, as long as you share the source
code and keep it under the same (or a compatible) license. For details, see [License](LICENSE).
