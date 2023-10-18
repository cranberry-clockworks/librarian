<script lang="ts">
    export let fetch: () => Promise<string>;
    
    let audio: HTMLAudioElement | undefined;
    let playing = false;
    
    async function toggle()
    {
        playing = !playing;
        
        if (playing)
        {
            if (audio === undefined)
            {
                let data = await fetch();
                audio = new Audio(data);
            }
            
            await audio.play();
        }
        else
        {
            audio?.pause();
        }
    }
</script>

<button on:click={toggle}>
    {#if playing}
        <span>⏸️</span>
    {:else}
        <span>▶️</span>
    {/if}
</button>