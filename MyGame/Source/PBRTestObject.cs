using GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------
class PBRTestObject : GameObject
{
	public PBRTestObject(string szModel, string szTextureName, Vector3 v3Pos)
	{
		MeshRenderer renderer = AddComponent<MeshRenderer>();
		
		Model model = new Model(szModel);
		renderer.SetModel(model);

		Material material = new Material("PBREffect");

		Texture albedo = new Texture(szTextureName + "_alb");
		if(albedo != null)
			material.SetProperty("_Albedo", albedo);

		Texture metalness = new Texture(szTextureName + "_met");
		if(metalness != null)
			material.SetProperty("_Metalness", metalness);

		Texture normal = new Texture(szTextureName + "_norm");
		if(normal != null)
			material.SetProperty("_Normal", normal);

		Texture roughness = new Texture(szTextureName + "_rough");
		if(roughness != null)
			material.SetProperty("_Roughness", roughness);

		Texture AO = new Texture(szTextureName + "_AO");
		if(AO != null)
			material.SetProperty("_AO", AO);

		//material.SetProperty("_Color", Color.White.ToVector3());

		renderer.SetMaterial(material);

		SetLocalPosition(v3Pos);
	}
}
