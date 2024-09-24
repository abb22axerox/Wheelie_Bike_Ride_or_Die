from PIL import Image, ImageDraw
 
# Load the uploaded image
image_path = 'Assets/Sprites/UI/Bigbutton.png'
image = Image.open(image_path)
 
# Create a drawing object
draw = ImageDraw.Draw(image)
 
# Define the coordinates where the text is located (this might require manual adjustment depending on the image)
# Here, we assume the text is in the middle of the image, and we want to fill the area where the text is with a color similar to the background.
text_box = (110, 90, 390, 150)  # This is an approximate rectangle for the text area
 
# Fill the area with a color similar to the surrounding background
draw.rectangle(text_box, fill=(182, 202, 212))  # Assuming the background is a light metallic gray
 
# Save the modified image
output_path = 'Assets/Sprites/New_Playbutton.png'
image.save(output_path)
 
output_path