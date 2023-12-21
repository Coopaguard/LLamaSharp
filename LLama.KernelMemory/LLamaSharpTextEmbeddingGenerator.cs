﻿using LLama;
using LLama.Abstractions;
using LLama.Common;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI;
using Microsoft.SemanticKernel.AI.Embeddings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLamaSharp.KernelMemory
{
    /// <summary>
    /// Provides text embedding generation for LLamaSharp.
    /// </summary>
    public class LLamaSharpTextEmbeddingGenerator
        : ITextEmbeddingGenerator, IDisposable
    {
        private readonly LLamaSharpConfig? _config;
        private readonly LLamaWeights? _weights;
        private readonly LLamaEmbedder _embedder;
        private bool _ownsEmbedder = false;
        private bool _ownsWeights = false;

        /// <inheritdoc/>
        public int MaxTokens => (int?)_config?.ContextSize ?? 2048;

        /// <summary>
        /// Initializes a new instance of the <see cref="LLamaSharpTextEmbeddingGenerator"/> class.
        /// </summary>
        /// <param name="config">The configuration for LLamaSharp.</param>
        public LLamaSharpTextEmbeddingGenerator(LLamaSharpConfig config)
        {
            this._config = config;
            var @params = new ModelParams(_config.ModelPath);
            _weights = LLamaWeights.LoadFromFile(@params);
            _embedder = new LLamaEmbedder(_weights, @params);
            _ownsWeights = true;
            _ownsEmbedder = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LLamaSharpTextEmbeddingGenerator"/> class from reused weights.
        /// </summary>
        /// <param name="config">The configuration for LLamaSharp.</param>
        /// <param name="weights">A LLamaWeights object.</param>
        public LLamaSharpTextEmbeddingGenerator(LLamaSharpConfig config, LLamaWeights weights)
        {
            this._config = config;
            var @params = new ModelParams(_config.ModelPath);
            _weights = weights;
            _embedder = new LLamaEmbedder(_weights, @params);
            _ownsEmbedder = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LLamaSharpTextEmbeddingGenerator"/> class from reused embedder.
        /// </summary>
        /// <param name="embedder">A LLamaEmbedder object.</param>
        public LLamaSharpTextEmbeddingGenerator(LLamaEmbedder embedder)
        {
            this._config = null;
            this._weights = null;
            _embedder = embedder;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_ownsWeights)
            {
                _weights?.Dispose();
            }
            if (_ownsEmbedder)
            {
                _embedder.Dispose();
            }
        }

        /// <inheritdoc/>
        public Task<IList<ReadOnlyMemory<float>>> GenerateEmbeddingsAsync(IList<string> data, CancellationToken cancellationToken = default)
        {
            IList<ReadOnlyMemory<float>> results = new List<ReadOnlyMemory<float>>();

            foreach (var d in data)
            {
                var embeddings = _embedder.GetEmbeddings(d);
                results.Add(new ReadOnlyMemory<float>(embeddings));
            }

            return Task.FromResult(results);
        }

        /// <inheritdoc/>
        public Task<Embedding> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
        {
            var embeddings = _embedder.GetEmbeddings(text);
            return Task.FromResult(new Embedding(embeddings));
        }

        /// <inheritdoc/>
        public int CountTokens(string text) => _embedder.Context.Tokenize(text).Length;
    }
}
