class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "3.0.0"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v3.0.0/tinycity-v3.0.0-osx-arm64"
      sha256 "6cd4e17cfbcae50aab9577841a0f6f0fde34af9e6443660b98dab862406d414e"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v3.0.0/tinycity-v3.0.0-osx-x64"
      sha256 "fa087b8390bd8277a9faa5d37609aa1c1dbcbdf7fbbf6d392c91239fc5b1015c"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v3.0.0/tinycity-v3.0.0-linux-x64"
    sha256 "b477e3698a2df6994f6f2c8714e3c8bfb7129903c0abd0524a9adcd88d286786"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v3.0.0-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v3.0.0-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v3.0.0-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end














