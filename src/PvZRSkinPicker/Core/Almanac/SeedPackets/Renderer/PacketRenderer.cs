namespace PvZRSkinPicker.Almanac.SeedPackets.Renderer;

using Il2CppReloaded.Gameplay;

using Il2CppSource.Controllers;

using PvZRSkinPicker.Assets;

using SolarApi.Unity.Extensions;

using UnityEngine;

internal sealed class PacketRenderer : IDisposable
{
    private readonly GameObject root;

    private readonly Camera camera;

    private readonly RenderTexture renderTexture;

    private bool disposed;

    private PacketRenderer(GameObject root, Camera camera, RenderTexture renderTexture)
    {
        this.root = root;
        this.camera = camera;
        this.renderTexture = renderTexture;
    }

    public static PacketRenderer Create(Vector2 position)
    {
        var root = new GameObject(nameof(PacketRenderer));
        root.transform.position = position;

        var seedPacketSprite = ModAssets.LoadSprite(ModAssets.GreenSeedPacket);

        var seedPacketRenderer = root.AddComponent<SpriteRenderer>();
        seedPacketRenderer.sprite = seedPacketSprite;
        seedPacketRenderer.sortingOrder = -10;

        var cameraObject = new GameObject("Camera");
        var cameraTransform = cameraObject.transform;
        cameraTransform.SetParent(root.transform, worldPositionStays: false);
        cameraTransform.localPosition = new Vector3(0, 0, -10);

        var camera = cameraObject.AddComponent<Camera>();
        camera.enabled = false;
        camera.orthographic = true;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.clear;

        float spriteHeight = seedPacketRenderer.sprite.bounds.size.y;
        camera.orthographicSize = spriteHeight * 0.5f;

        int width = seedPacketSprite.texture.width;
        int height = seedPacketSprite.texture.height;
        var renderTexture = new RenderTexture(width, height, depth: 0, RenderTextureFormat.ARGB32);
        renderTexture.Create();

        return new PacketRenderer(root, camera, renderTexture);
    }

    public void Dispose()
    {
        if (this.disposed)
        {
            return;
        }

        this.disposed = true;
        this.renderTexture.Release();
        Object.Destroy(this.renderTexture);
        Object.Destroy(this.root);
    }

    public Sprite RenderPlantToSprite(GameObject prefab, PacketRenderSpec<SeedType> renderSpec)
    {
        var texture = this.Render(prefab, SetupPlant, renderSpec);

        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f);
    }

    private static void SetupPlant(GameObject target, PacketRenderSpec<SeedType> renderSpec)
    {
        var controller = target.GetComponent<PlantController>();

        var plant = new Plant();

        controller.Init(plant);

        plant.PlantInitialize(0, 0, renderSpec.Type, SeedType.None, controller);

        var skelAnim = controller.AnimationController.m_skeletonAnimation;
        skelAnim.Update(0);
        skelAnim.LateUpdate();

        var targetT = target.transform;
        targetT.localPosition = new Vector2(renderSpec.Transform.X, renderSpec.Transform.Y);
        targetT.localScale = Vector2.one * (renderSpec.Transform.Scale * 0.025f);
    }

    private Texture2D Render<T>(
        GameObject prefab,
        Action<GameObject, PacketRenderSpec<T>> setupAction,
        PacketRenderSpec<T> renderSpec)
        where T : struct, Enum
    {
        ArgumentNullException.ThrowIfNull(prefab.Ref());

        var target = Object.Instantiate(prefab, this.root.transform, worldPositionStays: false);
        try
        {
            target.SetActive(true);
            setupAction.Invoke(target, renderSpec);
        }
        catch (Exception)
        {
            Object.DestroyImmediate(target);
            throw;
        }

        RenderTexture previousActive = RenderTexture.active;

        try
        {
            this.camera.targetTexture = this.renderTexture;
            this.camera.Render();
            RenderTexture.active = this.renderTexture;

            int width = this.renderTexture.width;
            int height = this.renderTexture.height;

            var texture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: true);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();

            return texture;
        }
        finally
        {
            this.camera.targetTexture = null;
            RenderTexture.active = previousActive;
            Object.DestroyImmediate(target);
        }
    }
}
