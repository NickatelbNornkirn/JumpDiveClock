# Documentation

## CLI usage

```text
JumpDiveClock [config_folder:<folder>] split:<split name>

config_folder: the folder where config.yml is ~/.config by default.
split: the name of the split you want to use, it is stored in the config_folder. The parts surrounded by [] are optional.
The parts surrounded by <> should be replaced with the appropriate text.


Examples:
JumpDiveClock split:cool_game
JumpDiveClock config_folder:$HOME/speedruns/ split:example
```

## Split configuration

## Local keybindings

Only work when the timer window is focused.

Esc - closes window.
L - locks timer (doesn't work after the run begins).


### Initial setup

I recommend you to copy and paste the [example](splits/example.yml) splits, modifying it to suit
your needs. Then consult this config if you aren't sure about what something does.

### Positions

Positions are always relative to whatever contains the object in question (e.g. a piece of text is
contained inside a segment).
X begins (i.e. is 0) at the leftmost part of the container and is 1 at the rightmost part.
Y begins at the top of the container and ends on its bottom.

### Values

WIP. Just look at the example config and figure stuff out. GLHF.

