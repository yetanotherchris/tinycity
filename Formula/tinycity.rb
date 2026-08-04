class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "3.3.0"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v3.3.0/tinycity-v3.3.0-osx-arm64"
      sha256 "e3a7f41201c67ae15a111de3c96a198f0343a4fa8d7ae43da15bc82285a0f9f2"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v3.3.0/tinycity-v3.3.0-osx-x64"
      sha256 "b614e47a67d1e767d5b4fbe7225f34019c54d1761111c70a2e74297f2f1ce641"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v3.3.0/tinycity-v3.3.0-linux-x64"
    sha256 "c5b21df2e19aa2a9b56d0de74b3021a107fe969ec7623cb50ee2d25f0b1bdc18"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v3.3.0-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v3.3.0-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v3.3.0-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end


















