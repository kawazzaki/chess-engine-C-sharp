extends Node2D

class_name  Piece

signal piece_selected(piece : Piece)

@onready  var icon_path = $icon
var slot_ID := -1

var type : int 


func load_icon(type):
	icon_path.texture = load(Datahandler.assets[type])


func _on_icon_gui_input(event: InputEvent) -> void:
	if event.is_action_pressed("mouse_left"):
		piece_selected.emit(self)
