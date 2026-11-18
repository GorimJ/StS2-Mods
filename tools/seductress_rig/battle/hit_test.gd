extends Node2D
# preview only: fire the Hit travel the way BaseLib does, at fixed times
var t := 0.0
var fired := 0
const AT := [1.0, 2.6]
func _process(d: float) -> void:
	t += d
	if fired < AT.size() and t >= AT[fired]:
		fired += 1
		var tree: AnimationTree = $Char.find_child("AnimationTree", true, false)
		print("has hit: ", tree.has_animation("hit"), "  active: ", tree.active)
		var pb: AnimationNodeStateMachinePlayback = tree.get("parameters/playback")
		pb.travel("hit", true)
