# JumpDiveClock

A speedrun timer for X. Intended for my personal use. I add features that seem useful to me. There
aren't many timers for Linux so I suppose that it could be useful for somebody else.

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
* locking the timer, preventing inputs from being triggered globally (useful if you want to do
    something else while having your speedrun timer open);
* running on Linux (and probably on *BSD, haven't tested it).

### Should I use this?

Probably not. It's still **extremely** unstable. Expect to make changes (and sometimes drastic ones)
to your config after every update.

### Where did this name come from?

I speedrun a Mario fangame where the fastest way to move without equipment is to repeatedly jump and
dive midair.

### Wayland support?

I don't plan on adding it myself because I don't use Wayland and don't plan on doing so in the short
term, but I'm open to pull requests.

### Other OSes?

I don't intend on doing ports, but I'm open to pull requests.

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

Jump dive clock is intended for more advanced users, but it shouldn't be too difficult for the less
experienced ones, assuming that you read the [documentation](DOCS.md) and see the
[example split](splits/example.yml) and the [example style](styles/example.yml).

And you are expected to manually compile it yourself, see #Running. If you aren't able to follow the
instructions for compiling, you probably should be using something else.

## Modification and redistribution

You are free to modify and/or redistribute copies of this software, as long as you share the source
code and keep it under the same (or a compatible) license. For details, see [License](LICENSE).
