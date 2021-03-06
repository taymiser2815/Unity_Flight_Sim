import sys, os
from osgeo import gdal

def main(img_list_path, in_ext, size):
    img_list = [os.path.join(root, filename)
          for root, dirnames, filenames in os.walk(img_list_path)
          for filename in filenames if filename.endswith('.'+in_ext)]
    for img in img_list:
        print img
        file_path = os.path.dirname(img)
        filename = os.path.basename(img).split(".")[0]
        cmd = "magick convert -resize " + str(size) + "x" + str(size) + " " + img + " " + img
        os.system(cmd)
    
if __name__ == "__main__":
    if(len(sys.argv) < 4):
        print("USAGE: python resize.py <input path> < input ext> < output size>")
        sys.exit(0)
    list_path = sys.argv[1]
    in_ext = sys.argv[2]
    size = sys.argv[3]

    main(list_path, in_ext, size)