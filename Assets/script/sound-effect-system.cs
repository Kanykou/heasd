using UnityEngine;

public class SoundEffectSystem : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource; // สำหรับเสียง One-shot
    [SerializeField] private AudioSource movementSource; // สำหรับเสียงต่อเนื่อง เช่น วิ่ง

    [Header("Movement SFX")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip wallJumpSound;
    [SerializeField] private AudioClip wallSlideSound;
    [SerializeField] private AudioClip wallGrabSound;
    [SerializeField] private AudioClip landingSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Volume Settings")]
    [Range(0, 1)]
    [SerializeField] private float sfxVolume = 0.7f;
    [Range(0, 1)]
    [SerializeField] private float movementVolume = 0.5f;

    private Movement movement;
    private Collision collision;
    private bool wasOnGround;
    private bool isWallSliding;

    private void Start()
    {
        // หา components ที่จำเป็น
        movement = GetComponent<Movement>();
        collision = GetComponent<Collision>();

        // ถ้ายังไม่มี AudioSource ให้สร้างใหม่
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        if (movementSource == null)
        {
            movementSource = gameObject.AddComponent<AudioSource>();
            movementSource.playOnAwake = false;
            movementSource.loop = true;
        }

        // ตั้งค่าเริ่มต้น
        sfxSource.volume = sfxVolume;
        movementSource.volume = movementVolume;
        wasOnGround = collision.onGround;
    }

    private void Update()
    {
        HandleJumpSounds();
        HandleWallSounds();
        HandleLandingSound();
    }

    private void HandleJumpSounds()
    {
        // ตรวจจับการกดปุ่มกระโดด
        if (Input.GetButtonDown("Jump"))
        {
            if (collision.onGround && jumpSound != null)
            {
                PlayOneShot(jumpSound);
            }
            else if (collision.onWall && !collision.onGround && wallJumpSound != null)
            {
                PlayOneShot(wallJumpSound);
            }
        }
    }

    private void HandleWallSounds()
    {
        // จัดการเสียงเกาะกำแพงและเลื่อนกำแพง
        if (movement.wallGrab && wallGrabSound != null)
        {
            if (!movementSource.isPlaying || movementSource.clip != wallGrabSound)
            {
                PlayLooping(wallGrabSound);
            }
        }
        else if (movement.wallSlide && wallSlideSound != null)
        {
            if (!isWallSliding)
            {
                PlayLooping(wallSlideSound);
                isWallSliding = true;
            }
        }
        else
        {
            if (movementSource.isPlaying)
            {
                movementSource.Stop();
            }
            isWallSliding = false;
        }
    }

    private void HandleLandingSound()
    {
        // ตรวจจับการลงพื้น
        if (!wasOnGround && collision.onGround)
        {
            if (landingSound != null)
            {
                PlayOneShot(landingSound);
            }
        }
        wasOnGround = collision.onGround;
    }

    public void PlayDeathSound()
    {
        if (deathSound != null)
        {
            PlayOneShot(deathSound);
        }
    }

    private void PlayOneShot(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    private void PlayLooping(AudioClip clip)
    {
        movementSource.clip = clip;
        movementSource.Play();
    }

    // ฟังก์ชั่นสำหรับปรับเสียง
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }

    public void SetMovementVolume(float volume)
    {
        movementVolume = Mathf.Clamp01(volume);
        movementSource.volume = movementVolume;
    }
}
