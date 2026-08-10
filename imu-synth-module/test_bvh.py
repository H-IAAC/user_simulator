from imusim.io.bvh import BVHLoader
import yaml

with open('config.yaml', 'r') as f:
    config = yaml.safe_load(f)

file_bvh = config['file_bvh']

with open(file_bvh, 'r') as bvhFile:
    loader = BVHLoader(bvhFile, 1)
    loader._readHeader()
    model = loader.model

# lista todos os joints do modelo carregado
for joint in model.joints:
    print(joint.name)