<script lang="ts">
    export let fetch: () => Promise<string>;
    export let title = "Pronounce";
    
    let audio: HTMLAudioElement | undefined;
    
    let playing = false
    
    async function toggle()
    {
        if (!playing)
        {
            if (audio === undefined)
            {
                let data = await fetch();
                audio = new Audio(data);
                audio.addEventListener(
                    'ended',
                    () => { playing = false; }
                )
            }
            
            playing = true;
            await audio.play();
        }
        else
        {
            playing = false;
            audio?.pause();
        }
    }
</script>

<button on:click={toggle}>
    {#if !playing}
        <span>{title}</span>
    {:else}
        <span>Pause</span>
    {/if}
</button>