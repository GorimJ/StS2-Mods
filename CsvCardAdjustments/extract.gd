extends SceneTree
func _init():
    var img = Image.load_from_file("res://images/atlases/power_atlas.png")
    if img:
        img.save_png("C:/Users/Gorim/Downloads/power_atlas_raw.png")
        print("Success")
    else:
        var tex = load("res://images/atlases/power_atlas.png")
        if tex:
            tex.get_image().save_png("C:/Users/Gorim/Downloads/power_atlas_raw.png")
            print("Success load")
        else:
            print("Failed")
    quit()
