class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "2.3.0"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.3.0/tinycity-v2.3.0-osx-arm64"
      sha256 "0c1c909ad542b02182a71e8ac52e5984d6d7a7da5da96ddc9272a3e7c699b07b"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.3.0/tinycity-v2.3.0-osx-x64"
      sha256 "1ed24c6bdafb793a8675c1e74c162ad126a25da5c331f7f444f3c8007acdde82"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v2.3.0/tinycity-v2.3.0-linux-x64"
    sha256 "7c6278b2616318d5a46be0e558ee21b47bbc5de9d19cf0a3a6c17515661918a0"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v2.3.0-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v2.3.0-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v2.3.0-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end











