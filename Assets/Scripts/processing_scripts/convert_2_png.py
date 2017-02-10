import sys, os
from osgeo import gdal

def main(img_list_path, out_ext):
    for file in img_list_path:
        file_path = os.path.dirname(file)
        filename = os.path.basename(file).split(".")[0]
        cmd = "magick convert -quiet " + file + " " + file_path + "/" + filename + "." + out_ext
        os.system(cmd)
    
if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("USAGE: python convert_2_png.py <lists of files> <out ext>")
        sys.exit(0)
    list_path = sys.argv[1]
    out_ext = sys.argv[2]
    
    
    
    list_path_list = open(list_path).readlines()
    list_path_list = map(lambda x:x.rstrip(),list_path_list)
    
    main(list_path_list, out_ext)