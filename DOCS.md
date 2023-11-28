# Documentation

## CLI usage

```text
JumpDiveClock [config_folder:<folder>] split:<split name>

config_folder: the folder where config.yml is ~/.config by default.
split: the name of the split you want to use, it is stored in the config_folder.

The parts surrounded by [] are optional.
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

#### attempt_size_text_pos_x

Type: fractional number (e.g. 7.5).

Relative horizontal position.

#### attempt_size_text_pos_y

Type: fractional number (e.g. 7.5).

#### attempt_count

Type: integer (e.g. 7).

How many attempts were completed in this split.

Updated automatically whenever a run is finished or at every reset.

#### attempt_count_font_size

Type: integer (e.g. 7).

How large is the font for the attempt counter.

#### attempt_count_font_spacing

Type: integer (e.g. 7).

How large is the spacing between the attempt counter's characters.

#### category

Type: text.

The name of the category that's the being ran.

#### category_title_font_size

#### category_title_font_spacing

#### extra_stats

#### game_name

#### game_title_font_size

#### game_title_font_spacing

#### header_height

#### hex_colors

#### background

#### pace_ahead_gaining

#### pace_ahead_losing

#### pace_behind_gaining

#### pace_behind_losing

#### pace_best

#### separator

#### text_base

#### max_segment_size

#### segment_font_size

#### segment_font_spacing

#### segment_margin

#### segments

#### List format

#### best_segment_time_rel

#### name

#### pb_time_rel

#### reset_count

#### segments_per_screen

#### separator_size

#### timer_font_size

#### timer_font_spacing

#### timer_size

#### title_category_titles_gap

#### world_record_seconds

#### Stats
