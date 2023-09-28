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

df.rename(columns={"Latitude":"x","Longitude":"y"}, inplace=True)

positionList = df[["x","y"]].to_dict(orient="records")
data = {"List":positionList, "notResetOnEnable": True, "itemCount":len(df), "type": "HIAAC.ScriptableList.Vector2SList"}

file = open(filename+"_locations.json", "w")
json.dump(data, file)
file.close()

times = df["Timestamp"].to_list()
data = {"List":times, "notResetOnEnable": True, "itemCount":len(df), "type": "HIAAC.ScriptableList.FloatSList"}

file = open(filename+"_times.json", "w")
json.dump(data, file)
file.close()