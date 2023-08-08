<script>
	import { getDefinition } from "$lib/dictionary/scrapper";
  </script>
  
  {#await getDefinition("gå")}
    <p>loading</p>
  {:then items}
    <p>{JSON.stringify(items)}</p>
  {:catch error}
    <p style="color: red">{error.message}</p>
  {/await}
  