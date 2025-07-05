
import requests

def query_ollama(prompt):
    response = requests.post(
        "http://localhost:11434/api/generate",
        json={
            "model": "deepseek-r1:1.5b",
            "prompt": prompt,
            "stream": False
        }
    )
    return response.json()["response"]
