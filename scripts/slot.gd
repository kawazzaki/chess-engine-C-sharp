extends TextureRect

class_name Slot

@onready var filter_path = $filter

var slot_ID := -1
var state = Datahandler.slot_states.NONE

signal  slot_clicked(slot)


func _ready() -> void:
	_set_filter()


func set_background(tex : Texture2D) -> void:
	texture = tex


func _set_filter(color = Datahandler.slot_states.NONE):
	state = color
	match  color:
		Datahandler.slot_states.NONE:
			filter_path.color = Color(1,1,1,0)
		Datahandler.slot_states.FREE:
			filter_path.color = Color(0,00,0,0.5)
	pass


func _on_filter_gui_input(event: InputEvent) -> void:
	if event.is_action_pressed("mouse_left"):
		slot_clicked.emit(self)
