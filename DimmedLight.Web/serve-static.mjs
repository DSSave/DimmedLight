import { createReadStream, existsSync, statSync } from "node:fs";
import { createServer } from "node:http";
import { extname, join, normalize, resolve } from "node:path";

const root = resolve("itch-publish", "wwwroot");
const port = Number(process.env.PORT ?? 5179);

const mimeTypes = new Map([
  [".html", "text/html; charset=utf-8"],
  [".js", "text/javascript; charset=utf-8"],
  [".css", "text/css; charset=utf-8"],
  [".json", "application/json; charset=utf-8"],
  [".wasm", "application/wasm"],
  [".dll", "application/octet-stream"],
  [".dat", "application/octet-stream"],
  [".xnb", "application/octet-stream"],
  [".png", "image/png"],
  [".ico", "image/x-icon"],
  [".ogg", "audio/ogg"],
]);

createServer((request, response) => {
  const url = new URL(request.url ?? "/", "http://localhost");
  const requestedPath = normalize(decodeURIComponent(url.pathname)).replace(/^(\.\.[/\\])+/, "");
  let filePath = join(root, requestedPath);

  if (!filePath.startsWith(root)) {
    response.writeHead(403);
    response.end("Forbidden");
    return;
  }

  if (!existsSync(filePath) || statSync(filePath).isDirectory()) {
    filePath = join(root, "index.html");
  }

  const contentType = mimeTypes.get(extname(filePath)) ?? "application/octet-stream";
  response.writeHead(200, { "Content-Type": contentType });
  createReadStream(filePath).pipe(response);
}).listen(port, () => {
  console.log(`Serving Dimmed Light from ${root} at http://localhost:${port}`);
});
