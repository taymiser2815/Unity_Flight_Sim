import sys, os
from osgeo import gdal

def main(img_list_path, output_path,tile_sise, slice_amount, south, north, west, east):
    os.system('gdalbuildvrt temp.vrt -input_file_list ' + img_list_path )
    for s_bound in range(south, north):
        for w_bound in range(west, east):
            lat = 'N'
            long = 'E'
            if south < 0:
                lat = 'S'
            if west < 0:
                long = 'W'
            geocell = ("%s%02d%s%03d" % (lat, abs(s_bound), long, abs(w_bound)))
            os.system("mkdir %s\\%s" % (output_path, geocell))
            for x in range(0,slice_amount):
                os.system("mkdir " + output_path + "\\" + geocell + "\\" + str(x))
                for y in range (0, slice_amount):
                    x_min_slice = float(w_bound) + float(float(x)/slice_amount)
                    x_max_slice = float(w_bound) + float(float(x + 1)/slice_amount)
                    y_min_slice = float(s_bound) + float(float(y)/slice_amount)
                    y_max_slice = float(s_bound) + float(float(y + 1)/slice_amount)
                    os.system("gdalwarp -te " + str(x_min_slice) + " " + str(y_min_slice) + " " + str(x_max_slice) + " " + str(y_max_slice) + " -ts " + str(tile_sise) + " " + str(tile_sise) + " -of GTiff temp.vrt " + output_path + "/" + geocell + "/" + str(x) + "/" + str(x) + "_" + str(y) + ".tif" )
    
    # close dataset
    ds = None
    
if __name__ == "__main__":
    if(len(sys.argv) < 9):
        print("USEAGE: python slicendice.py <list of geo referenced img> <output path> <south bound> <nouth bound> <west bound> <east bound> <slice amount> <image size>")
        sys.exit(0)
    list_path = sys.argv[1]
    output = sys.argv[2]
    south = int(sys.argv[3])
    north = int(sys.argv[4])
    west = int(sys.argv[5])
    east = int(sys.argv[6])
    slice_amount = int(sys.argv[7])
    tile_sise = int(sys.argv[8])
    gdalbuildvrt = 0.0000152587890625
    
    
    
    #list_path_list = open(list_path).readlines()
    #list_path_list = map(lambda x:x.rstrip(),list_path_list)
    
    main(list_path, output, tile_sise, slice_amount, south, north, west, east)