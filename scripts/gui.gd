extends Control



@onready var slot_scene = preload("res://scenes/slot.tscn")
@onready var board_grid : GridContainer = $chessBoard/boadGrid
@onready var piece_scene = preload("res://scenes/piece.tscn")
@onready var chess_board : ColorRect = $chessBoard
@onready var bitboard = $bitboard
@onready var GeneratePath = $GeneratePath
@onready var ChessBot = $ChessBot

@onready var slot_tex = preload("res://assets/new/square brown light_png_shadow_512px.png")


var grid_array : Array[Slot] = []

var piece_array : Array[Piece] = []

var icon_offset : Vector2 = Vector2(39,39)

var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"

var piece_selected : Piece = null

var gameStart := false

func _ready() -> void:
	for i in range(64):
		create_slot()

	for i in range(8):
		for j in range(8):
			if (i + j) % 2 == 0:
				grid_array[i * 8 + j].set_background(slot_tex)
	
	piece_array.resize(64)
	piece_array.fill(null)
	
	#start game
	reset_game()
	
func create_slot():
	var new_slot : Slot = slot_scene.instantiate()
	new_slot.slot_ID = grid_array.size()
	board_grid.add_child(new_slot)
	grid_array.push_back(new_slot)
	new_slot.slot_clicked.connect(_on_slot_clicked)

func _on_slot_clicked(slot : Slot):
	if not piece_selected:return
	if slot.state != Datahandler.slot_states.FREE:return
	var moved_piece_type = piece_selected.type   # capture before piece_selected is cleared
	move_piece(piece_selected,slot.slot_ID)
	piece_selected = null
	clear_board_filter()
	
	if gameStart:
		var bot_is_black = moved_piece_type < 6
		var result = ChessBot.call("FindNextMove", bot_is_black)
		var move = PackedInt32Array(result)

		if move.size() < 2:
			gameStart = false
			if bitboard.call("IsCheckmate", bot_is_black):
				print("Checkmate! You win.")
			else:
				print("Stalemate — draw.")
			reset_game()
			return

		update_board(63-move[0],63-move[1])
	
		var human_is_black = not bot_is_black
		if bitboard.call("IsCheckmate", human_is_black):
			gameStart = false
			print("Checkmate! Bot wins.")
			reset_game()

func update_board(from_loc, to_loc) -> void:
	if piece_array[to_loc]:
		if piece_array[to_loc].type%6 == 1:
			gameStart = false
			print("King captured — game over.")
			reset_game()
			return
		piece_array[to_loc].queue_free()
		piece_array[to_loc] = null
		
	var tween = create_tween()
	tween.tween_property(
		piece_array[from_loc],
		"global_position",
		grid_array[to_loc].global_position + icon_offset,
		0.2
	)
	piece_array[to_loc] = piece_array[from_loc]
	piece_array[from_loc] = null
	piece_array[to_loc].slot_ID = to_loc




func move_piece(piece, location) -> void:
	if piece_array[location] == piece:return
	if piece_array[location]:
		remove_from_bitboard(piece_array[location])
		piece_array[location].queue_free()
		piece_array[location] = null

	piece_array[piece.slot_ID] = null
	remove_from_bitboard(piece)
	bitboard.call("AddPiece",63-location,piece.type)
	# Move visually
	var tween = create_tween()
	tween.tween_property(
		piece,
		"global_position",
		grid_array[location].global_position + icon_offset,
		0.2
	)

	# Update array
	piece_array[location] = piece
	piece.slot_ID = location

func remove_from_bitboard(piece : Piece):
	bitboard.call("RemovePiece",63-piece.slot_ID,piece.type)


func add_piece(piece_type , location)-> void:
	var new_piece : Piece  = piece_scene.instantiate()
	chess_board.add_child(new_piece)
	new_piece.type = piece_type
	new_piece.load_icon(piece_type)
	new_piece.global_position = grid_array[location].global_position + icon_offset
	piece_array[location] = new_piece
	new_piece.slot_ID = location
	
	new_piece.piece_selected.connect(_on_piece_selected)



func _on_piece_selected(piece):
	if piece_selected:
		_on_slot_clicked(grid_array[piece.slot_ID])
	else:
		piece_selected = piece
		var WhiteBoard = bitboard.call("GetWhiteBitBoard")
		var BlackBoard = bitboard.call("GetBlackBitBoard")
		#check for color
		var isBlack := true
		var function_to_call : String
		if piece.type<6: isBlack = false 
		#match piece type
		match piece.type%6:
			0:
				function_to_call = "BishopPath"
			1:
				function_to_call = "KingPath"
			2:
				function_to_call = "KnightPath"
			3:
				function_to_call = "PawnPath"
			4:
				function_to_call = "QueenPath"
			5:
				function_to_call = "RookPath"
		if isBlack:
			set_board_filter(GeneratePath.call(function_to_call,63 - piece.slot_ID, BlackBoard, WhiteBoard, isBlack))
		else:
			set_board_filter(GeneratePath.call(function_to_call,63 - piece.slot_ID, WhiteBoard, BlackBoard, isBlack))

func set_board_filter(bitmap:int):
	for i in range(64):
		if bitmap & 1:
			grid_array[63-i]._set_filter(Datahandler.slot_states.FREE)
		bitmap = bitmap >> 1

func parse_fen(fen : String):
	var board_state = fen.split(" ")
	var board_index := 0
	for i in board_state[0]:
		if i == "/":continue
		if i.is_valid_int():
			board_index += i.to_int()
		else:
			add_piece(Datahandler.fen_dict[i],board_index)
			board_index += 1


func clear_board_filter():
	for i in grid_array:
		i._set_filter()


func _on_button_pressed() -> void:
	parse_fen(fen)
	bitboard.call("InitBitBoard",fen)
	


func _on_button_2_pressed() -> void:
	reset_game()


func reset_game():
	clear_board_filter()
	clear_piece_array()
	piece_selected = null
	parse_fen(fen)
	bitboard.call("InitBitBoard",fen)
	ChessBot.call("InitBot",bitboard)
	gameStart = true

func clear_piece_array():
	for i in piece_array:
		if i:
			i.queue_free()
	piece_array.fill(null)


func _process(delta: float) -> void:
	if Input.is_action_just_pressed("mouse_right") && piece_selected:
		piece_selected = null
		clear_board_filter()
