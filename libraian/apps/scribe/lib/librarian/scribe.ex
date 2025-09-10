defmodule Librarian.Scribe do
  def main(argv) do
    with {:ok, template_path} <- parse_arguments(argv),
         {:ok, template} <- File.read(template_path),
         input <- IO.read(:stdio, :eof),
         {:ok, entries} <- Jason.decode(input),
         true <- is_list(entries) do
      for entry <- entries do
        EEx.eval_string(template, assigns: entry) |> IO.puts()
      end
    else
      {:error, reason} -> IO.puts(reason)
    end
  end

  defp parse_arguments(argv) do
    case OptionParser.parse(argv, strict: [template: :string]) do
      {[template: path], _, []} -> {:ok, path}
      {_, _, [errors]} -> {:error, errors}
      _ -> {:error, argv}
    end
  end
end
