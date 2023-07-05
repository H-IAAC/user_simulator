import json
import argparse
import os

import pandas as pd

parser = argparse.ArgumentParser()
parser.add_argument("path")
args = parser.parse_args()

path : str = args.path

filename = os.path.basename(path).split(".")[0]

df = pd.read_csv(path)

df.sort_values("Timestamp", inplace=True)

start_time = df["Timestamp"].min()
df["Timestamp"] -= start_time
df["Timestamp"] /= 1000

df.rename(columns={"Timestamp":"z", "Latitude":"x","Longitude":"y"}, inplace=True)

List = df[["x","y","z"]].to_dict(orient="records")

data = {"List":List, "notResetOnEnable": True, "itemCount":len(df), "type": "HIAAC.ScriptableList.GeoSList"}

file = open(filename+".json", "w")
json.dump(data, file)
file.close()