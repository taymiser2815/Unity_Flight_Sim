import sys, os
from osgeo import gdal

def main(img_list_path, in_ext):
    img_list = [os.path.join(root, filename)
          for root, dirnames, filenames in os.walk(img_list_path)
          for filename in filenames if filename.endswith('.'+in_ext)]
    for img in img_list:
        print img
        file_path = os.path.dirname(img)
        filename = os.path.basename(img).split(".")[0]
        cmd = "magick convert " + img + " -depth 16 gray:" + file_path+"/"+filename+".raw"
        os.system(cmd)
    
if __name__ == "__main__":
    if len(sys.argv < 3):
        print("USAGE: python convert_2_raw_16_bit.py <input dir> <img ext>")
        sys.exit(0)
    list_path = sys.argv[1]
    in_ext = sys.argv[2]

    main(list_path, in_ext)