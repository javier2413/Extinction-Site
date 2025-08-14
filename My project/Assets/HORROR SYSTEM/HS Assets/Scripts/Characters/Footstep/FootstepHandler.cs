using UnityEngine;
using System.Collections.Generic;

public class FootstepHandler : MonoBehaviour
{
    [Header("Footstep Settings")]
    public AudioSource footstepAudioSource;
    public LayerMask groundLayer;
    public float raycastDistance = 1f;

    [Header("Footstep Group")]
    public List<FootstepGroup> footstepGroups = new List<FootstepGroup>();

    private Dictionary<Texture2D, FootstepGroup> footstepGroupsByTexture;

    private void Start()
    {
        InitializeFootstepGroups();
    }

    private void InitializeFootstepGroups()
    {
        footstepGroupsByTexture = new Dictionary<Texture2D, FootstepGroup>();
        foreach (var group in footstepGroups)
        {
            foreach (var texture in group.surfaceTextures)
            {
                if (!footstepGroupsByTexture.ContainsKey(texture))
                {
                    footstepGroupsByTexture.Add(texture, group);
                }
            }
        }
    }

    private Texture2D GetSurfaceTexture()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, groundLayer))
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                return renderer.material.mainTexture as Texture2D;
            }
        }
        return null;
    }

    // This is the method your animation will call
    public void PlayFootstep()
    {
        Texture2D texture = GetSurfaceTexture();
        FootstepGroup group = null;

        if (texture != null && footstepGroupsByTexture.TryGetValue(texture, out group))
        {
            if (group.audioClips.Length > 0)
            {
                int index = Random.Range(0, group.audioClips.Length);
                footstepAudioSource.PlayOneShot(group.audioClips[index], group.footstepVolume);
            }
        }
        else if (footstepGroups.Count > 0)
        {
            // fallback: pick a random footstep from the first group
            FootstepGroup fallbackGroup = footstepGroups[0];
            if (fallbackGroup.audioClips.Length > 0)
            {
                int index = Random.Range(0, fallbackGroup.audioClips.Length);
                footstepAudioSource.PlayOneShot(fallbackGroup.audioClips[index], fallbackGroup.footstepVolume);
            }
        }
    }

    [System.Serializable]
    public class FootstepGroup
    {
        public List<Texture2D> surfaceTextures = new List<Texture2D>();
        public AudioClip[] audioClips;
        [Range(0, 1)] public float footstepVolume = 1.0f;
    }
}

