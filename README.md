# JumpDiveClock

A speedrun timer for X. Intended for my personal use. I add features that seem useful to me.

## Installation

### Dependencies

* xinput (should come with X11).

## FAQ

### Why?

I tried some timers for Linux and I either disliked them or they didn't work at all. So I made my
own. And made this to share to other people that may find it useful.

### What features do you have?

* marking time (duh);
* comparing the current time with your personal best;
* having stats (best possible time, current pace, % of runs that reach each segment, personal best,
    world record, sum of best);
* having a backup system that creates copies of the speedgame's data whenever a change is made to
    it;
* being almost completely customizable via a config file;
* running on Linux (and probably on *BSD, haven't tested it).

### Where did this name come from?

I speedrun a Mario fangame where the fastest way to move without equipment is to repeatedly jump and
dive.

## Wayland?

I don't plan on adding it myself because I don't use Wayland and don't plan on doing so, but I'm
open to pull requests.

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
