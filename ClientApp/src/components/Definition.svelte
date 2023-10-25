<script lang="ts">
    import type {NounDefinition, UnknownDefinition, VerbDefinition} from "$lib/generated/client";
    import NounDefinitionBody from "./NounDefinitionBody.svelte";
    import VerbDefinitionBody from "./VerbDefinitionBody.svelte";
    import UnknownDefinitionBody from "./UnknownDefinitionBody.svelte";
    import {cards} from "$lib/state";
    
    export let definition: NounDefinition | VerbDefinition | UnknownDefinition
</script>
<div>
    {#if definition.$type === "Noun" }
        <NounDefinitionBody definition={definition}/>
    {:else if definition.$type === "Verb"}
        <VerbDefinitionBody definition={definition}/>
    {:else }
        <UnknownDefinitionBody definition={definition}/>
    {/if}
    <button on:click={() => cards.update(c => [...c, {title: "sss", front: "front", back: "back", media: ["s"]}])}>Add</button>
</div>

<style>
    div {
        border: thin solid cornflowerblue;
        padding: 10px 20px;
        border-radius: 5px;
        margin-bottom: 10px;
    }
</style>

