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

WIP. Just look at the example config and figure stuff out. GLHF.

### Positions

Positions are always relative to whatever contains the object in question.
X begins (i.e. is 0) at the leftmost part of the container.
Y begins at the top of the container.

Example:

```none

(x: 0.4, y: 0.6)
00000
00000
0#000
00000
00000

```

### attempt_size_text_pos_x

Type: fractional number (e.g. 7.5).

Relative horizontal position.

### attempt_size_text_pos_y

Type: fractional number (e.g. 7.5).

### attempt_count

Type: integer (e.g. 7).

How many attempts were completed in this split.

### attempt_count_font_size

How big is the font for the attempt count.

### attempt_count_font_spacing

How large is the font font for the attempt count.

### category

### category_title_font_size

### category_title_font_spacing

### extra_stats

### game_name

### game_title_font_size

### game_title_font_spacing

### header_height

### hex_colors

#### background

#### pace_ahead_gaining

#### pace_ahead_losing

#### pace_behind_gaining

#### pace_behind_losing

#### pace_best

#### separator

#### text_base

### max_segment_size

### segment_font_size

### segment_font_spacing

### segment_margin

### segments

#### List format

#### best_segment_time_rel

#### name

#### pb_time_rel

#### reset_count

### segments_per_screen

### separator_size

### timer_font_size

### timer_font_spacing

### timer_size

### title_category_titles_gap

### world_record_seconds

## Stats
