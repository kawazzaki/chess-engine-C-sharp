extends Node


var assets := []

enum PieceNames {
	WHITE_BISHOP, 
	WHITE_KING, 
	WHITE_KNIGHT, 
	WHITE_PAWN, 
	WHITE_QUEEN, 
	WHITE_ROOK, 
	BLACK_BISHOP, 
	BLACK_KING, 
	BLACK_KNIGHT, 
	BLACK_PAWN, 
	BLACK_QUEEN, 
	BLACK_ROOK
}

var fen_dict := {
	"b": PieceNames.BLACK_BISHOP,
	"k": PieceNames.BLACK_KING,
	"n": PieceNames.BLACK_KNIGHT,
	"p": PieceNames.BLACK_PAWN,
	"q": PieceNames.BLACK_QUEEN,
	"r": PieceNames.BLACK_ROOK,
	"B": PieceNames.WHITE_BISHOP,
	"K": PieceNames.WHITE_KING,
	"N": PieceNames.WHITE_KNIGHT,
	"P": PieceNames.WHITE_PAWN,
	"Q": PieceNames.WHITE_QUEEN,
	"R": PieceNames.WHITE_ROOK,
}

enum slot_states {NONE,FREE}

func _ready() -> void:
	assets.append("res://assets/new/b_bishop_png_shadow_512px.png")
	assets.append("res://assets/new/b_king_png_shadow_512px.png")
	assets.append("res://assets/new/b_knight_png_shadow_512px.png")
	assets.append("res://assets/new/b_pawn_png_shadow_512px.png")
	assets.append("res://assets/new/b_queen_png_shadow_512px.png")
	assets.append("res://assets/new/b_rook_png_shadow_512px.png")
	assets.append("res://assets/new/w_bishop_png_shadow_512px.png")
	assets.append("res://assets/new/w_king_png_shadow_512px.png")
	assets.append("res://assets/new/w_knight_png_shadow_512px.png")
	assets.append("res://assets/new/w_pawn_png_shadow_512px.png")
	assets.append("res://assets/new/w_queen_png_shadow_512px.png")
	assets.append("res://assets/new/w_rook_png_shadow_512px.png")
