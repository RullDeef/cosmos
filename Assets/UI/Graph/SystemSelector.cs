using System.Collections.Generic;
using UnityEngine;

/**
 * Синглтон-класс для управления системами.
 * 
 * Мониторит, какие системы выделены в данный момент времени,
 * подсвечивая их. (не реализовано)
 */
public class SystemSelector : MonoBehaviour
{
    /**
     * Сам список выбранных систем.
     */
    public List<PlanetSystem> selected;

    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        // render process here...
    }
}