<script lang="ts">
    import {createEventDispatcher} from "svelte";
    import type {Card} from "$lib/models";
    

    export let card: Card;
    const dispatch = createEventDispatcher();
    
    function removeButtonClicked()
    {
        dispatch("removeButtonClicked", card.title);
    }
    
</script>

<div>
    <span>{card.title}</span>
    <button on:click={removeButtonClicked}>Remove</button>
</div>

<style>
    button {
        visibility: hidden;
    }
    div:hover button{
        visibility: visible;
    }
</style>